<div align="center">

# 🏸 Regina Court Booking Bot

### ⚡ Automates court booking on Regina Active Communities

![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=for-the-badge&logo=windows)
![Framework](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![Browser](https://img.shields.io/badge/Automation-Playwright-2EAD33?style=for-the-badge&logo=playwright)
![Status](https://img.shields.io/badge/Status-Active-success?style=for-the-badge)
[![Download Release](https://img.shields.io/badge/⬇%20Download-Latest%20Release-black?style=for-the-badge)](https://github.com/nguyentuongvinh2911/regina-court-booking-bot/releases/latest)

</div>

---

## 📌 Quick Navigation

- [🚀 Quick Start (non-technical users)](#quick-start)
- [✅ What this bot does](#what-it-does)
- [📦 Package contents](#package-contents)
- [🛠️ What the scripts do](#scripts)
- [⚙️ Where to edit settings](#settings-files)
- [📚 Settings reference](#settings-reference)
- [❓ FAQ](#faq)
- [👨‍💻 Developer commands](#dev-commands)
- [🔒 Security note](#security)

<a id="quick-start"></a>
## 🚀 Quick Start (non-technical users)

> [!TIP]
> Run `setup.bat` only once per machine, then use `run.bat` for each booking attempt.

1. Download the latest Windows zip from GitHub Releases.
2. Unzip to a folder (example: `C:\ReginaCourtBookingBot`).
3. Run `setup.bat` once.
4. Edit `secrets.json` with your account credentials.
5. Edit `appsettings.json` with your booking preferences.
6. Run `run.bat` whenever you want to book.

---

<a id="what-it-does"></a>
## ✅ What this bot does

- Opens booking page
- Signs in
- Picks date
- Chooses slot (primary + fallbacks)
- Fills details and waivers
- Reserves (or stops early in dry-run mode)

---

<a id="package-contents"></a>
## 📦 Package contents (after unzip)

```
ReginaCourtBookingBot/
  ReginaCourtBookingBot.exe
  appsettings.json
  secrets.json
  setup.bat
  run.bat
  playwright.ps1
  ...runtime files (.dll, etc.)
```

<a id="scripts"></a>
## 🛠️ What the scripts do

### 🧩 `setup.bat` (run once per PC)

- Installs Playwright Chromium browser using `playwright.ps1 install chromium`
- Shows a clear error if browser install fails
- Does **not** change your settings files

### ▶️ `run.bat` (run every booking)

- Starts `ReginaCourtBookingBot.exe`
- Prints logs to the console
- Waits at the end so users can read success/error output

---

<a id="settings-files"></a>
## ⚙️ Where to edit settings

### 🔐 `secrets.json` (credentials only)

```json
{
  "AppSettings": {
    "Username": "your-login-name",
    "Password": "your-password"
  }
}
```

### 🧠 `appsettings.json` (behavior and booking preferences)

Edit this file for schedule, slot priorities, and booking details.

---

<a id="settings-reference"></a>
## 📚 Settings reference

### 🧭 `AppSettings`

- `BaseUrl`: booking page URL. Keep default unless site URL changes.
- `Username`, `Password`: keep empty here; use `secrets.json`.
- `AllowedRunDays`: allowed weekdays, comma-separated. Example: `"Monday,Friday"`.
- `RunAtLocalTime`: optional app start time (`HH:mm` or `HH:mm:ss`). Empty = start immediately.
- `ReviewButtonClickAtLocalTime`: optional exact time for review/confirm click. Empty = click immediately.
- `StrictReviewButtonClickTime`: `true` = fail if late; `false` = continue if late.
- `Headless`: `false` visible browser, `true` background browser.
- `DryRun`: `true` stops before final reserve click and saves screenshot.
- `SlowMoMilliseconds`: delay between actions (`0`, `50`, `100` common).
- `Selectors`: advanced UI locators. Usually do not edit.

### 🗓️ `BookingRequest`

- `Date`: fixed booking date. If empty, `DateOffsetDays` is used.
- `DateOffsetDays`: number of days from today (used only when `Date` is empty).
  - Current default is `3` because this booking system opens courts 3 days ahead.
  - If your booking system opens earlier or later, change this value.
- `CourtType`: `"Badminton"` or `"Tennis"`.
- `SlotLabel`: first-choice slot label.
- `SlotFallbacks`: backup slots, tried in order.
- `EventName`: text entered in Event Name.
- `Player2FullName`: text for second-player field.

### 🎯 Slot selection behavior

The bot tries:
1. `SlotLabel`
2. each entry in `SlotFallbacks` (top to bottom)

If none are available, booking stops safely.

---

<a id="faq"></a>
## ❓ FAQ

### The browser window did not open. Is that normal?
> ✅ Yes. If `Headless=true`, it runs in the background. Set `Headless=false` to see the browser.

### Why did it wait instead of starting now?
> ⏰ If `RunAtLocalTime` is set, the app waits until that local time.

### Why did it stop before final reservation?
> 🧪 `DryRun=true` intentionally stops before the final reserve click. Set `DryRun=false` for real booking.

### What if my first slot is unavailable?
> 🔁 It tries `SlotFallbacks` in order. Reorder these to match your priority.

### Where do I put my username/password?
> 🔐 Only in `secrets.json`. Keep `appsettings.json` credentials empty.

### Can I share my configured package with others?
> 📤 Yes, but remove real credentials first. Share a blank `secrets.json` template.

---

<a id="dev-commands"></a>
## 👨‍💻 Developer commands

```powershell
# Run app
dotnet run --project src/ReginaCourtBookingBot/ReginaCourtBookingBot.csproj

# Run tests
dotnet test tests/ReginaCourtBookingBot.Tests/ReginaCourtBookingBot.Tests.csproj

# Build distributable
./publish.ps1
```

---

<a id="security"></a>
## 🔒 Security note

> [!WARNING]
> Never commit real credentials.
>
> Share only a blank `secrets.json` template.