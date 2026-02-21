using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;

namespace HelloClipboard.Utils
{
    public static class CryptoHelper
    {
        private static byte[] _masterKey;

        // Load the Master Key from the Registry or create and save it if it doesn't exist
        private static byte[] GetMasterKey()
        {
            if (_masterKey != null) return _masterKey;

            // STEP 1: Attempt to read from the Registry
            try
            {
                // Looking under HKCU (HKEY_CURRENT_USER). This does not require administrator privileges.
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(AppConstants.RegistryKeyPath))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(AppConstants.RegistryKeyValueName);
                        if (value != null && value is byte[] encryptedKey)
                        {
                            // Decrypt using DPAPI (Only the current Windows user can decrypt this)
                            _masterKey = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);
                            return _masterKey;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Registry Read Error: {ex.Message}");
#endif
                // If a reading or decryption error occurs (corrupted data), a new key will be generated below.
            }

            // STEP 2: Create a NEW key if it doesn't exist or is corrupted
            byte[] newKey = new byte[32]; // 256-bit
            RandomNumberGenerator.Fill(newKey);

            // STEP 3: Encrypt the new key with DPAPI and save it to the Registry
            try
            {
                byte[] keyToSave = ProtectedData.Protect(newKey, null, DataProtectionScope.CurrentUser);

                // CreateSubKey: Creates the key if it doesn't exist, or opens it if it does (in Write mode)
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(AppConstants.RegistryKeyPath))
                {
                    if (key != null)
                    {
                        key.SetValue(AppConstants.RegistryKeyValueName, keyToSave, RegistryValueKind.Binary);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Registry Write Error: {ex.Message}");
#endif
                // Even if writing to the Registry fails, we can continue using the key in memory (newKey) for the current session.
                // However, the key will be lost when the application closes; therefore, this step is critical.
            }

            _masterKey = newKey;
            return _masterKey;
        }

        public static byte[] Encrypt(byte[] data)
        {
            if (data == null || data.Length == 0) return null;

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = GetMasterKey();
                    aes.GenerateIV(); // Random IV for each encryption session

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    {
                        // Prepend the IV to the beginning of the data
                        ms.Write(aes.IV, 0, aes.IV.Length);

                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            cs.FlushFinalBlock();
                        }
                        return ms.ToArray();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static byte[] Decrypt(byte[] data)
        {
            if (data == null || data.Length == 0) return null;

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = GetMasterKey();

                    // Get the IV size (usually 16 bytes)
                    byte[] iv = new byte[aes.BlockSize / 8];

                    if (data.Length < iv.Length) return null; // Data is too short, cannot be encrypted

                    // Extract the IV from the beginning of the data
                    Array.Copy(data, 0, iv, 0, iv.Length);
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                        {
                            // Decrypt the part after the IV (the actual encrypted content)
                            cs.Write(data, iv.Length, data.Length - iv.Length);
                            cs.FlushFinalBlock();
                        }
                        return ms.ToArray();
                    }
                }
            }
            catch (CryptographicException)
            {
                // If decryption fails (e.g., an unencrypted file from an older version),
                // return the data as-is for backward compatibility.
                return data;
            }
            catch
            {
                return null;
            }
        }
    }
}