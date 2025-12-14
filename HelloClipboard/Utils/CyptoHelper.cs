using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;

namespace HelloClipboard.Utils
{
	public static class CryptoHelper
	{
		private static byte[] _masterKey;

		// Master Key'i Registry'den yükle veya yoksa oluşturup Registry'e yaz
		private static byte[] GetMasterKey()
		{
			if (_masterKey != null) return _masterKey;

			// 1. ADIM: Registry'den okumayı dene
			try
			{
				// HKCU (HKEY_CURRENT_USER) altına bakıyoruz. Yönetici izni gerektirmez.
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Constants.RegistryKeyPath))
				{
					if (key != null)
					{
						object value = key.GetValue(Constants.RegistryKeyValueName);
						if (value != null && value is byte[] encryptedKey)
						{
							// DPAPI kullanarak şifreyi çöz (Sadece bu Windows kullanıcısı çözebilir)
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
				// Okuma hatası veya şifre çözme hatası olursa (bozuk data), aşağıda yenisini oluşturacağız.
			}

			// 2. ADIM: Anahtar yoksa veya bozuksa YENİSİNİ OLUŞTUR
			byte[] newKey = new byte[32]; // 256-bit
			using (var rng = new RNGCryptoServiceProvider())
			{
				rng.GetBytes(newKey);
			}

			// 3. ADIM: Yeni anahtarı DPAPI ile şifrele ve Registry'e kaydet
			try
			{
				byte[] keyToSave = ProtectedData.Protect(newKey, null, DataProtectionScope.CurrentUser);

				// CreateSubKey: Anahtar yoksa oluşturur, varsa açar (Yazma modunda)
				using (RegistryKey key = Registry.CurrentUser.CreateSubKey(Constants.RegistryKeyPath))
				{
					if (key != null)
					{
						key.SetValue(Constants.RegistryKeyValueName, keyToSave, RegistryValueKind.Binary);
					}
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine($"Registry Write Error: {ex.Message}");
#endif
				// Registry'e yazamazsak bile hafızadaki (newKey) ile o anlık çalışmaya devam edebiliriz,
				// ama program kapanınca anahtar kaybolur. Bu yüzden burası kritiktir.
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
					aes.GenerateIV(); // Her şifreleme için rastgele IV

					using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
					using (var ms = new MemoryStream())
					{
						// IV'yi verinin başına ekle
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

					// IV boyutunu al (Genelde 16 byte)
					byte[] iv = new byte[aes.BlockSize / 8];

					if (data.Length < iv.Length) return null; // Veri çok kısa, şifreli olamaz

					// IV'yi verinin başından ayır
					Array.Copy(data, 0, iv, 0, iv.Length);
					aes.IV = iv;

					using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
					using (var ms = new MemoryStream())
					{
						using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
						{
							// IV'den sonraki kısmı (şifreli asıl veriyi) çöz
							cs.Write(data, iv.Length, data.Length - iv.Length);
							cs.FlushFinalBlock();
						}
						return ms.ToArray();
					}
				}
			}
			catch (CryptographicException)
			{
				// Eğer şifre çözülemezse (eski versiyondan kalan şifresiz dosya ise)
				// olduğu gibi döndür (Geriye uyumluluk)
				return data;
			}
			catch
			{
				return null;
			}
		}
	}
}