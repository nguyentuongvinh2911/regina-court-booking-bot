# Regina Court Booking Bot

Automates court booking on Regina Active Communities.

## Quick Start (1 minute)

1. Download latest zip from GitHub Releases.
2. Unzip it anywhere (example: `C:\ReginaCourtBookingBot`).
3. Double-click `setup.bat` (one-time only).
4. Open `secrets.json` and enter your username/password.
5. Open `appsettings.json` and confirm your slot preferences.
6. Double-click `run.bat` to run the booking bot.

If you only read one section, read this one first.

## What this bot does

- Opens the Regina booking page
- Signs in with your account
- Picks your target date
- Tries your preferred slot first, then fallback slots in order
- Fills booking details and waiver checkboxes
- Clicks Reserve (or stops early in dry-run mode)

---

## For non-technical users (recommended)

You do **not** need Visual Studio or .NET installed.

### What to download from GitHub

From **Releases**, download the latest Windows zip package (example: `ReginaCourtBookingBot-win-x64.zip`).

# Regina Court Booking Bot

Automates court booking on Regina Active Communities.

## Quick Start (non-technical users)

1. Download the latest Windows zip from GitHub Releases.
2. Unzip to a folder (example: `C:\ReginaCourtBookingBot`).
3. Run `setup.bat` once.
4. Edit `secrets.json` with your account credentials.
5. Edit `appsettings.json` with your booking preferences.
6. Run `run.bat` whenever you want to book.

---

## What this bot does

- Opens booking page
- Signs in
- Picks date
- Chooses slot (primary + fallbacks)
- Fills details and waivers
- Reserves (or stops early in dry-run mode)

---

## Package contents (after unzip)

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

## What the scripts do

### `setup.bat` (run once per PC)

- Installs Playwright Chromium browser using `playwright.ps1 install chromium`
- Shows a clear error if browser install fails
- Does **not** change your settings files

### `run.bat` (run every booking)

- Starts `ReginaCourtBookingBot.exe`
- Prints logs to the console
- Waits at the end so users can read success/error output

---

## Where to edit settings

### `secrets.json` (credentials only)

```json
{
  "AppSettings": {
    "Username": "your-login-name",
    "Password": "your-password"
  }
}
```

### `appsettings.json` (behavior and booking preferences)

Edit this file for schedule, slot priorities, and booking details.

---

## Settings reference

### `AppSettings`

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

### `BookingRequest`

- `Date`: fixed booking date. If empty, `DateOffsetDays` is used.
# Regina Court Booking Bot

Automates court booking on Regina Active Communities.

## Quick Start (non-technical users)

1. Download the latest Windows zip from GitHub Releases.
2. Unzip to a folder (example: `C:\ReginaCourtBookingBot`).
3. Run `setup.bat` once.
4. Edit `secrets.json` with your account credentials.
5. Edit `appsettings.json` with your booking preferences.
6. Run `run.bat` whenever you want to book.

---

## What this bot does

- Opens booking page
- Signs in
- Picks date
- Chooses slot (primary + fallbacks)
- Fills details and waivers
- Reserves (or stops early in dry-run mode)

---

## Package contents (after unzip)

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

## What the scripts do

### `setup.bat` (run once per PC)

- Installs Playwright Chromium browser using `playwright.ps1 install chromium`
- Shows a clear error if browser install fails
- Does **not** change your settings files

### `run.bat` (run every booking)

- Starts `ReginaCourtBookingBot.exe`
- Prints logs to the console
- Waits at the end so users can read success/error output

---

## Where to edit settings

### `secrets.json` (credentials only)

```json
{
  "AppSettings": {
    "Username": "your-login-name",
    "Password": "your-password"
  }
}
```

### `appsettings.json` (behavior and booking preferences)

Edit this file for schedule, slot priorities, and booking details.

---

## Settings reference

### `AppSettings`

- `BaseUrl`: booking page URL. Keep default unless site URL changes.
- `Username`, `Password`: keep empty here; use `secrets.json`.
- `RunAtLocalTime`: optional app start time (`HH:mm` or `HH:mm:ss`). Empty = start immediately.
- `ReviewButtonClickAtLocalTime`: optional exact time for review/confirm click. Empty = click immediately.
- `StrictReviewButtonClickTime`: `true` = fail if late; `false` = continue if late.
- `Headless`: `false` visible browser, `true` background browser.
- `DryRun`: `true` stops before final reserve click and saves screenshot.
- `SlowMoMilliseconds`: delay between actions (`0`, `50`, `100` common).
- `Selectors`: advanced UI locators. Usually do not edit.

### `BookingRequest`

- `Date`: fixed booking date. If empty, `DateOffsetDays` is used.
- `DateOffsetDays`: number of days from today (used only when `Date` is empty).
  - Current default is `3` because this booking system opens courts 3 days ahead.
  - If your booking system opens earlier or later, change this value.
- `CourtType`: `"Badminton"` or `"Tennis"`.
- `SlotLabel`: first-choice slot label.
- `SlotFallbacks`: backup slots, tried in order.
- `EventName`: text entered in Event Name.
- `Player2FullName`: text for second-player field.

### Slot selection behavior

The bot tries:
1. `SlotLabel`
2. each entry in `SlotFallbacks` (top to bottom)

If none are available, booking stops safely.

---

## FAQ

### The browser window did not open. Is that normal?
Yes. If `Headless=true`, it runs in the background. Set `Headless=false` to see the browser.

### Why did it wait instead of starting now?
If `RunAtLocalTime` is set, the app waits until that local time.

### Why did it stop before final reservation?
`DryRun=true` intentionally stops before the final reserve click. Set `DryRun=false` for real booking.

### What if my first slot is unavailable?
It tries `SlotFallbacks` in order. Reorder these to match your priority.

### Where do I put my username/password?
Only in `secrets.json`. Keep `appsettings.json` credentials empty.

### Can I share my configured package with others?
Yes, but remove real credentials first. Share a blank `secrets.json` template.

---

## Developer commands

- Run app: `dotnet run --project src/ReginaCourtBookingBot/ReginaCourtBookingBot.csproj`
- Run tests: `dotnet test tests/ReginaCourtBookingBot.Tests/ReginaCourtBookingBot.Tests.csproj`
- Build distributable: `./publish.ps1`

---

## Security note

- Never commit real credentials.
- Share only blank `secrets.json` template.