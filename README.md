# HelloClipboard

> A lightweight Windows clipboard history manager that captures, searches, and previews your clipboard content instantly.

## 🚀 Features

- **Automatic History** – records every text copy without user intervention.
- **Instant Search** – type to filter past items in real‑time.
- **Detail Preview** – open a dedicated window with raw content and horizontal scrolling.
- **Tray Integration** – runs silently; access via system tray icon.
- **Custom Settings** – tweak history size, auto‑start, UI behavior, and clear history.

## 📸 Screenshots

<img width="852" height="735" alt="image" src="https://github.com/user-attachments/assets/da4547d2-5ebe-4395-beba-95868b8c896d" />

## ⚙️ Installation

```bash
# Download the latest installer from the releases page
# Then run HelloClipboard_Installer.exe
```

### Auto‑start

The app can be configured to launch at Windows startup via its settings dialog.

## 📚 Developer Guide

| Role | Where to start |
|------|----------------|
| **Build** | `HelloClipboard.sln` – use Visual Studio 2026 or newer. |
| **Code Structure** | - `Core/TrayApplicationContext.cs` – main entry point.- `Services/ClipboardMonitor.cs` – core clipboard logic.- `Views/*` – UI forms and controls. |
| **Configuration** | `HelloClipboard/Constants/Constants.cs` – shared constants and paths.`latest_version.json` – version metadata. |
| **Hotkeys & Privileges** | `TrayApplicationContext.ReloadGlobalHotkey()` handles hotkey registration; see `Utils/PriviligesHelper.cs`. |

### Running Locally

```bash
dotnet build HelloClipboard.sln
```

The built executable is in `HelloClipboard/bin/Debug/net10.0-windows/`.

## 📄 License

MIT – see the [LICENSE](LICENSE) file.

---

**Contributing**
Feel free to open issues or pull requests. Please follow the PR template in `.github/pull_request_template.md` for consistency.

