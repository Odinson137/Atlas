# Atlas Shuttle Monitor 🚐💨

**Atlas** is an automated shuttle booking monitoring system developed to solve a real-life problem for a friend struggling to book highly demanded bus tickets. It periodically queries and scrapes shuttle booking platforms (such as **Atlasbus** and **Minsk-Lida**), playing instant audio alerts in the browser and sending direct Telegram notifications with booking links the moment free seats become available.

---

## 📄 Short Description
> **Atlas** is a Blazor-based web application and Telegram bot that monitors shuttle availability between Belarusian cities (Minsk, Lida, Berezovka, Braslav) across multiple booking services. It automatically scans for free seats on a customizable schedule, alerts the user with browser sounds, and broadcasts booking links via Telegram to ensure users can secure tickets before they sell out.

---

## ✨ Key Features
- **Dual Booking Service Integration**:
  - **Atlasbus** API integration (`atlasbus.by`) with support for location-to-city mapping.
  - **Minsk-Lida** scraper (`bilet.minsk-lida.by`) utilizing `HtmlAgilityPack` to parse active trip listings from dynamically rendered HTML.
- **Smart Filtering**: Filter rides by travel date, departure/arrival time slots, number of passengers, and selected booking platforms.
- **Telegram Bot Notifications**: Integrates with the Telegram Bot API to register users and send instant notifications containing direct links to book flights.
- **Browser Sound Alerts**: Plays an audio alert in the browser window as soon as a new trip or free seat is found.
- **Self-Configuring SQLite Database**: Uses Entity Framework Core with SQLite to store registered Telegram chats. The database tables are automatically generated on application startup.
- **Interactive Blazor UI**: Built with Blazor Interactive Server, offering a clean dashboard to manage search parameters, swap routes, track search status, and preview found trips in a table.

---

## 🛠️ Technology Stack
- **Runtime & Language**: .NET 8 (C#)
- **Web UI Framework**: ASP.NET Core & Blazor Web App (Interactive Server render mode)
- **Database**: SQLite (via Entity Framework Core 9.0)
- **Scraping**: HtmlAgilityPack (for parsing Minsk-Lida HTML data)
- **Bot Engine**: `Telegram.Bot` NuGet package
- **Client Communication**: `HttpClient` (for making external API calls)

---

## 📁 Repository Structure
```
Atlas/
├── Atlas/
│   ├── Components/            # Blazor UI components & layout (App.razor, Home.razor, etc.)
│   ├── Data/                  # DbContext setup (ApplicationDbContext.cs)
│   ├── Interfaces/            # Service definitions (IRouteSearcher, ITelegramService, etc.)
│   ├── Models/                # Data Transfer Objects (DTOs) and DB models (Models.cs, TelegramChat.cs)
│   ├── Properties/            # Launch profiles and settings
│   ├── Repositories/          # DB access implementations (TelegramChatRepository.cs)
│   ├── Services/              # Core business logic (AtlasRouteSearcher, TelegramService, etc.)
│   ├── wwwroot/               # Static assets (styles, alert sounds, scripts)
│   ├── Program.cs             # Application entry point and DI configuration
│   ├── appsettings.json       # App configuration parameters
│   └── Atlas.csproj           # MSBuild project file with dependencies
├── Atlas.sln                  # Visual Studio / Rider solution file
└── README.md                  # This file
```

---

## ⚙️ Configuration

To configure the application, open the `Atlas/appsettings.json` file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=contacts.db"
  },
  "Telegram": {
    "BotToken": "YOUR_TELEGRAM_BOT_TOKEN"
  }
}
```

1. **Telegram Bot Token**: Generate a bot token from [@BotFather](https://t.me/BotFather) on Telegram and paste it into `YOUR_TELEGRAM_BOT_TOKEN`.
2. **SQLite Database**: The database file `contacts.db` will be created automatically in the project folder at runtime. No manual migrations are required.

---

## 🚀 Getting Started

### Prerequisites
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

### Running the App
1. Clone this repository (if you haven't already):
   ```bash
   git clone https://github.com/Odinson137/Atlas.git
   cd Atlas
   ```
2. Navigate to the project folder containing the source code:
   ```bash
   cd Atlas
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Open your browser and navigate to `http://localhost:5000` (or the URL printed in the terminal console) to access the interactive dashboard.

---

## 🤖 Telegram Bot Commands
Once your bot is running, users can interact with it using these commands:
- `/start` - Registers the user's Telegram chat and enables automated notifications.
- `/stop` - Unsubscribes the user and disables notifications.

When the web dashboard is running a search and finds a valid trip, the bot broadcasts a formatted message to all active `/start` subscribers:
```text
Найдены новые маршрутки!
Маршрут: Минск → Лида
Дата: 2026-06-25
Найдено поездок: 1
Новая поездка: 18:30: [Перейти к маршруту](https://bilet.minsk-lida.by)
```
