# CampusFix - Campus Complaint & Issue Tracker

A mini-project built with **ASP.NET Core MVC (.NET 8)**, **Entity Framework Core** and **MySQL**.
Students report campus problems (hostel, mess, Wi-Fi, classrooms...) and admins track and resolve them.

## Features
- Student sign up / login (hashed passwords, session based auth)
- Raise, view, edit and delete complaints (full CRUD)
- Category, priority, location and status tracking
- Search, filter and pagination
- Comments / activity timeline on every complaint
- Admin dashboard: stats, resolution rate, category breakdown, urgent list
- Admin status updates with remarks
- Dark mode (cookie), flash messages (TempData), login state (Session)
- Responsive Bootstrap 5 UI

## Requirements
- .NET 8 SDK (or newer; change `TargetFramework` and package versions accordingly)
- MySQL Server 8.x running locally

## Run it
1. Open `appsettings.json` and put your MySQL password in the connection string.
2. In a terminal inside the project folder:
   ```
   dotnet restore
   dotnet run
   ```
3. Open the URL shown in the terminal (e.g. https://localhost:5001).

The database `campusfix_db`, its tables and demo data are created automatically on first run.

## Demo logins
| Role    | Email                 | Password    |
|---------|-----------------------|-------------|
| Admin   | admin@campusfix.com   | Admin@123   |
| Student | student@campusfix.com | Student@123 |

## Project structure
```
Controllers/   Home, Account, Complaints, Admin
Models/        User, Complaint, Comment, Enums
ViewModels/    Form + dashboard view models (validation lives here)
Data/          AppDbContext (EF Core), DbSeeder
Filters/       SessionAuthAttribute (role based access)
Helpers/       UiHelper (badges, icons)
Views/         Razor views + shared layout
wwwroot/       CSS and JS
```

## Optional: use EF migrations instead of EnsureCreated
```
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```
(Then remove `db.Database.EnsureCreated();` from Program.cs and call `db.Database.Migrate();`.)
