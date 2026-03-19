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
- [🕒 Run modes](#run-modes)
- [⚙️ Where to edit settings](#settings-files)
- [🧪 How to set up your settings](#settings-setup)
- [📚 Settings reference](#settings-reference)
- [❓ FAQ](#faq)
- [👨‍💻 Developer commands](#dev-commands)
- [🔒 Security note](#security)

<a id="quick-start"></a>
## 🚀 Quick Start (non-technical users)

> [!TIP]
> Run `setup.bat` only once per machine, then use `run-once.bat`, `run-service.bat`, or the menu in `run.bat`.

1. Download the latest Windows zip from GitHub Releases.
2. Unzip to a folder (example: `C:\ReginaCourtBookingBot`).
3. Run `setup.bat` once.
4. Edit `secrets.json` with your account credentials.
5. Edit either `appsettings.once.json` or `appsettings.service.json`.
6. Run `run-once.bat` or `run-service.bat`.

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
  appsettings.once.json
  appsettings.service.json
  secrets.json
  setup.bat
  run.bat
  run-once.bat
  run-service.bat
  playwright.ps1
  ...runtime files (.dll, etc.)
```

<a id="scripts"></a>
## 🛠️ What the scripts do

### 🧩 `setup.bat` (run once per PC)

- Installs Playwright Chromium browser using `playwright.ps1 install chromium`
- Shows a clear error if browser install fails
- Does **not** change your settings files

### ▶️ `run.bat` (easy menu)

- Opens a simple menu so you can choose one-time mode or scheduled mode
- Calls the dedicated launcher for the mode you choose

### ▶️ `run-once.bat`

- Runs one booking attempt, then exits
- Uses `appsettings.once.json`
- Saves logs to the `logs` folder

### ▶️ `run-service.bat`

- Starts the long-running scheduled worker mode
- Uses `appsettings.service.json`
- Saves logs to the `logs` folder

---

<a id="run-modes"></a>
## 🕒 Run modes

### One-time mode

- Use `run-once.bat`
- The bot runs once, using the current booking settings, then exits
- If `RunAtLocalTime` is set, it waits until that time before starting

### Scheduled service mode

- Use `run-service.bat`
- The bot keeps running and books automatically on the configured days/time
- Service mode uses both `AllowedRunDays` and `RunAtLocalTime`
- To stop it when running in a console window, press `Ctrl+C`

### Easy menu mode

- Double-click `run.bat`
- Pick option `1` for one-time mode or option `2` for scheduled mode
- This is the easiest option for non-technical users

> [!TIP]
> For a true Windows background service, install the app with a command like:
>
> `sc.exe create ReginaCourtBookingBot binPath= "\"C:\\Path\\To\\ReginaCourtBookingBot.exe\" --service" start= auto`
>
> For true Windows Service usage, set `Headless=true` because services should not depend on a visible desktop browser.

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

### 🧠 `appsettings.once.json` (one-time mode settings)

Edit this file when using `run-once.bat`.

### 🧠 `appsettings.service.json` (scheduled mode settings)

Edit this file when using `run-service.bat`.

---

<a id="settings-setup"></a>
## 🧪 How to set up your settings

### Step 1: Add your login in `secrets.json`

Put only your Regina account login in `secrets.json`:

```json
{
  "AppSettings": {
    "Username": "your-login-name",
    "Password": "your-password"
  }
}
```

### Step 2: Choose your run mode

Pick the settings file that matches how you want to run the bot.

### Example A: `appsettings.once.json`

Use this when you want to start the bot manually and let it run once.

```json
{
  "AppSettings": {
    "AllowedRunDays": "",
    "RunAtLocalTime": "08:59:55",
    "ReviewButtonClickAtLocalTime": "09:00:00",
    "StrictReviewButtonClickTime": true,
    "Headless": false,
    "DryRun": false,
    "SlowMoMilliseconds": 50
  },
  "BookingRequest": {
    "Date": "",
    "DateOffsetDays": 3,
    "CourtType": "Badminton",
    "SlotLabel": "FH - Badminton Court 1 8:00 PM - 9:00 PM Available",
    "SlotFallbacks": [
      "FH - Badminton Court 1 7:00 PM - 8:00 PM Available",
      "FH - Badminton Court 3 8:00 PM - 9:00 PM Available"
    ],
    "EventName": "1",
    "Player2FullName": "Second Player Name"
  }
}
```

- Start it with `run-once.bat`
- If `RunAtLocalTime` is empty, it starts immediately
- If `RunAtLocalTime` is set, it waits until that time, runs once, then exits

### Example B: `appsettings.service.json`

Use this when you want the bot to stay running and book automatically on specific days.

```json
{
  "AppSettings": {
    "AllowedRunDays": "Monday,Wednesday,Friday",
    "RunAtLocalTime": "08:59:55",
    "ReviewButtonClickAtLocalTime": "09:00:00",
    "StrictReviewButtonClickTime": true,
    "Headless": true,
    "DryRun": true,
    "SlowMoMilliseconds": 0
  },
  "BookingRequest": {
    "Date": "",
    "DateOffsetDays": 3,
    "CourtType": "Badminton",
    "SlotLabel": "FH - Badminton Court 1 8:00 PM - 9:00 PM Available",
    "SlotFallbacks": [
      "FH - Badminton Court 1 7:00 PM - 8:00 PM Available",
      "FH - Badminton Court 3 8:00 PM - 9:00 PM Available"
    ],
    "EventName": "1",
    "Player2FullName": "Second Player Name"
  }
}
```

- Start it with `run-service.bat`
- The process stays alive and waits for the next matching day/time
- Recommended for this mode:
  - `Headless=true` (required for a true Windows Service install)
  - `SlowMoMilliseconds=0`
  - `DryRun=false` only after you have verified everything works

### Step 3: Adjust the booking date behavior

- Leave `BookingRequest.Date` empty to use `DateOffsetDays`
- Use `DateOffsetDays=3` if bookings open 3 days ahead
- If you want a fixed date instead, set `Date` to a value like `2026-03-25`

### Step 4: Configure slot priority

- Put your first-choice slot in `SlotLabel`
- Put backup choices in `SlotFallbacks`
- The bot tries them from top to bottom

### Step 5: Test safely first

Before doing a real reservation:

- Set `DryRun=true`
- Set `Headless=false` so you can watch the browser
- Run one manual test with `run-once.bat`
- When satisfied, change `DryRun=false`

---

<a id="settings-reference"></a>
## 📚 Settings reference

### 🧭 `AppSettings`

- `BaseUrl`: booking page URL. Keep default unless site URL changes.
- `Username`, `Password`: keep empty here; use `secrets.json`.
- `AllowedRunDays`: allowed weekdays, comma-separated. Example: `"Monday,Friday"`.
- `RunAtLocalTime`: one-time mode waits until this time; service mode uses it as the daily scheduled run time.
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

### How do I make it run automatically on certain days?
> 🗓️ Set `AllowedRunDays` and `RunAtLocalTime` in `appsettings.service.json`, then start `run-service.bat`.

### Can it run as a real Windows background service?
> 🪟 Yes. The app supports `--service`, so you can register `ReginaCourtBookingBot.exe --service` with Windows Service Manager.

### Why did it stop before final reservation?
> 🧪 `DryRun=true` intentionally stops before the final reserve click. Set `DryRun=false` for real booking.

### What if my first slot is unavailable?
> 🔁 It tries `SlotFallbacks` in order. Reorder these to match your priority.

### Where do I put my username/password?
> 🔐 Only in `secrets.json`. Keep the credentials in both settings files empty.

### Can I share my configured package with others?
> 📤 Yes, but remove real credentials first. Share a blank `secrets.json` template.

---

<a id="dev-commands"></a>
## 👨‍💻 Developer commands

```powershell
# Run app once
dotnet run --project src/ReginaCourtBookingBot/ReginaCourtBookingBot.csproj

# Run scheduled service mode
dotnet run --project src/ReginaCourtBookingBot/ReginaCourtBookingBot.csproj -- --service

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