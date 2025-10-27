#Knowledge Base Dashboard API

A bilingual back-end API for managing articles, user profiles, statistics, and working hours in a centralized knowledge base system. Built to support a Next.js front-end with full authentication, personalization, and modular data access.

---

##Features

- User Authentication & Profiles
  - Secure login and profile management
  - Language and theme preferences (English/Arabic, Light/Dark)

- Articles Management
  - Create, edit, delete, reorder articles
  - Metadata: title, category, tags, image, publish status, schedule
  - Export articles for sharing or backup

- Statistics & Analytics
  - Aggregated article data by category and time
  - Date-range filtering for trends and engagement

- Working Hours Management
  - Weekly schedule per user with multiple time slots per day
  - Conflict detection for overlapping slots
  - Unsaved changes warning

- User Settings
  - Persistent language and theme preferences
  - Instant reflection on front-end

- Additional Features
  - Export reports (article summary, schedule overview)
  - Social media sharing

---

##🛠 Technologies Used

ASP.NET Core
Entity Framework Core
SQL Server
JWT Authentication
FluentValidation
Swagger / OpenAPI
Global Exception Handling
Response Compression (Gzip + Brotli)
In-Memory Caching
API Versioning

---

##Installation

bash
git clone https://github.com/sobhi2006/ArticleTask.git
cd ArticleTask
dotnet restore
dotnet run
