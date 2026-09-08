# Charity & Campaign Management Platform (Admin Panel)

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-WebAPI_%26_RazorPages-2C8EBB)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-ORM-blue)
![MSSQL](https://img.shields.io/badge/SQL_Server-2019%2B-CC292B?logo=microsoftsqlserver&logoColor=white)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2F%20N--Tier-brightgreen)
![UI](https://img.shields.io/badge/UI_Template-Vuexy-orange)

A comprehensive, scalable, and modular **Charity & Campaign Management Solution** designed to orchestrate charities, donation campaigns, category tagging, social profiles, and role-based administrative workflows.

---

## 🏛 Solution Architecture

The solution follows clean architectural separation and domain-driven practices divided into 5 focused layers:
```text
├── 📂 AdminPanel.Core            # Domain Models, Enums & Status State Helpers
├── 📂 AdminPanel.Application     # DTOs, Business Interfaces, Services, Security
├── 📂 AdminPanel.Infrastructure  # EF Core DbContext, Repositories, JWT & Storage
├── 📂 AdminPanel.Api             # RESTful API Endpoints & Auth Controllers
└── 📂 AdminPanel.UI              # Razor Pages Web UI with Vuexy Theme & AJAX Modals



✨ Key Features
Charity Management: Full profile orchestration with branding assets (logos, banners) and city lookups.
Campaign Workflow Engine: Lifecycle management for campaigns with role-based status transitions (CampaignStatus).
Social & Category Tagging: Many-to-many bindings (CharityCategory, SocialCharity) for dynamic social channels and categorizations.
Role-Based Access Control (RBAC): Admin users hierarchy (AdminUserType), secure password hashing, and token-based JWT infrastructure.
File Management Subsystem: Centralized media handling (IFileStorageService) for logos and campaign banners.
Rich Dashboard UI: Built on top of Vuexy admin dashboard with responsive DataTables, AJAX-driven CRUD modals, and toast notifications.




## 🛠 Tech Stack

*   **Framework:** .NET 8 (C#)
*   **Backend:** ASP.NET Core Web API & Razor Pages
*   **Data Access:** Entity Framework Core (Repository Pattern)
*   **Database:** Microsoft SQL Server
*   **Authentication:** JWT (JSON Web Tokens) & Custom Claim-based Authorization
*   **Frontend / UI:** Razor Pages, Bootstrap 5, Vuexy Admin Template, JavaScript (Async/Fetch APIs)
`






## 🚀 Getting Started

### 1. Prerequisites

*   **.NET 8.0 SDK**
*   **SQL Server 2019+** or **LocalDB**
*   **Visual Studio 2022** or **VS Code**

### 2. Database Setup

*   Open **SQL Server Management Studio (SSMS)**.
*   Execute the setup script located at `Database/CharityAdminDb_Script.sql` to generate tables and seed data.

### 3. Configuration

*   Duplicate `AdminPanel.Api/appsettings.Example.json` as `appsettings.json`.
*   Update your local SQL connection string in the `ConnectionStrings.DefaultConnection` field.
`


`` `json
{
  "ConnectionStrings": {
"DefaultConnection": "Server=localhost;Database=CharityAdminDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
`` `





### 4. Run the Project

You can set multiple startup projects in Visual Studio, or run them manually from your terminal:

# Run API
`$ dotnet run --project AdminPanel.Api` 

# Run Admin Web UI
`$ dotnet run --project AdminPanel.UI`




## 👨‍💻 Author & Contributions

Developed with precision and clean code principles.

Feel free to open [issues](../../issues) or submit [pull requests](../../pulls)! 🤝
`
