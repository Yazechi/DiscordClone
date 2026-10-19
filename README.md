# Discord Clone (.NET / WPF / SignalR)

A modular, real-time messaging application modeled after Discord. Built using **.NET 8/9**, **C#**, **WPF**, and **ASP.NET Core**, this project follows **Clean Architecture** principles to maintain strict separation of concerns between domain logic, data persistence, API endpoints, and client UI.

---

## 🛠️ Tech Stack & Architecture

- **Language & Framework:** C# / .NET 8.0 & 9.0
- **Backend API:** ASP.NET Core Web API with Swagger / OpenAPI
- **Real-Time WebSockets:** ASP.NET Core SignalR
- **Desktop Client:** WPF (Windows Presentation Foundation) with XAML
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core (EF Core)
- **Authentication:** JWT (JSON Web Token) Bearer Tokens & BCrypt Password Hashing

### Project Structure (Clean Architecture)

```
DiscordClone/
├── DiscordClone.Domain/          # Core Domain Entities, Enums, and Interfaces
├── DiscordClone.Infrastructure/  # AppDbContext, EF Core Migrations, Database Config
├── DiscordClone.Application/     # DTOs, Business Logic Services, Token Service
├── DiscordClone.API/             # Controllers, SignalR Hubs, API Pipelines
└── DiscordClone.DesktopClient/   # WPF Windows UI (Login, Register, Chat Views)
```

---

## ✨ Features Implemented

- **Modular Architecture:** Layered solution isolating Domain models from API and Infrastructure logic.
- **Database Schema & Migrations:** Complete relational database schema created via EF Core for Users, Guilds (Servers), Channels, Roles, Guild Members, Messages, Reactions, and Attachments.
- **JWT Authentication:** Secure user registration and login flow utilizing BCrypt password hashing and custom JWT issuance.
- **Real-Time Messaging:** Bidirectional WebSockets via SignalR `ChatHub` supporting live channel message broadcasting, channel grouping, and typing indicators.
- **Custom Desktop UI:** Dark-mode styled WPF desktop client featuring custom windows for authentication, dynamic server sidebar navigation, live channel switching, and real-time message collection binding.

---

## 🚀 Getting Started

### Prerequisites

1. [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
2. [PostgreSQL](https://www.postgresql.org/download/) running locally or in Docker on port `5432`.
3. [Visual Studio 2022](https://visualstudio.microsoft.com/) with the **.NET Desktop Development** and **ASP.NET and web development** workloads installed.

### Installation & Setup

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/YOUR_USERNAME/DiscordClone.git
   cd DiscordClone
   ```

2. **Configure Database Connection:**
   Update `appsettings.json` inside `DiscordClone.API` with your PostgreSQL user credentials:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=DiscordCloneDb;Username=postgres;Password=YOUR_POSTGRES_PASSWORD"
   }
   ```

3. **Apply EF Core Migrations:**
   Open Visual Studio **Package Manager Console** and execute:
   ```powershell
   Update-Database -Project DiscordClone.Infrastructure -StartupProject DiscordClone.API
   ```

4. **Run the Application:**
   - Set `DiscordClone.API` as the Startup Project and press `F5` to launch the API and Swagger UI on `https://localhost:7232`.
   - Right-click `DiscordClone.DesktopClient` in Solution Explorer $\rightarrow$ **Debug** $\rightarrow$ **Start New Instance** to launch the WPF app.

---

## 📜 License

This project is open-source and available under the [MIT License](LICENSE).