# Course Enrollment System

A Windows Forms desktop application for managing university course enrollment — courses, students, enrollments, and announcements. Built with C# / .NET 10 and Microsoft SQL Server using raw ADO.NET for all data access.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Database Setup](#database-setup)
- [Getting Started](#getting-started)
- [License](#license)

---

## Features

- **Courses** — Add, edit, view, delete, search, and filter courses by department and status (Open, Full, Cancelled). Includes seat tracking, a live pie chart of course status distribution, and business-rule validation (e.g. status/seat consistency checks, blocking deletion of courses with active enrollment history).
- **Students** — Manage student records including name, phone, email, and department. Full CRUD with search support and validation (name length, phone format, email format). Deletion is blocked while a student has active enrollments.
- **Enrollments** — Enroll students in courses with automatic seat and status updates. Tracks enrollment status (Enrolled, Completed, Dropped), records an optional grade, prevents enrolling in courses that are Cancelled/Full/out of seats, and locks a Completed or Dropped record from being reverted.
- **Announcements** — Create, edit, delete, and toggle active/inactive announcements, with an "Active Only" filter.
- **Dashboard** — Overview of total courses, students, active/completed/dropped enrollment counts, open courses, and a live feed of active announcements.
- **Search & Filter** — Text search combined with department/status dropdowns (Courses), text search (Students), and a status filter (Enrollments).
- **Status Bar** — Displays the current view name and timestamp in the main window, updated on every navigation action.

---

## Tech Stack

| Component | Technology |
|---|---|
| Language | C# (.NET 10) |
| UI Framework | Windows Forms (`net10.0-windows`) |
| Database | Microsoft SQL Server |
| Data Access | ADO.NET (`Microsoft.Data.SqlClient`) |
| Charting | `WinForms.DataVisualization` |
| Configuration | `System.Configuration.ConfigurationManager` |

---

## Project Structure

```
CourseEnrollmentSystem/
├── CourseEnrollment.Core/              # Class library — models, contracts, services
│   ├── CourseEnrollment.Core.csproj
│   ├── Contracts/
│   │   IAnnouncementService.cs
│   │   ICourseService.cs
│   │   IEnrollmentService.cs
│   │   IStudentService.cs
│   ├── Models/
│   │   Announcement.cs
│   │   Course.cs
│   │   Enrollment.cs
│   │   Student.cs
│   ├── Services/
│   │   DBAnnouncementService.cs
│   │   DBCourseService.cs
│   │   DBEnrollmentService.cs
│   │   DBStudentService.cs
│   └── Utilities/
│       CourseStatusEnum.cs
│       DepartmentEnum.cs
│       EnrollmentStatusEnum.cs
│
├── CourseEnrollment.WindowsApp/         # WinForms UI application
│   ├── CourseEnrollment.WindowsApp.csproj
│   ├── App.config                       # Database connection string
│   ├── Program.cs                       # Application entry point
│   ├── Forms/
│   │   CourseForm.cs
│   │   CoursePicker.cs
│   │   EnrollmentForm.cs
│   │   FormModeEnums.cs
│   │   MainForm.cs
│   │   StudentForm.cs
│   │   StudentPicker.cs
│   ├── Properties/
│   │   Resources.resx
│   ├── Resources/
│   │   
│   └── Views/
│       AnnouncementsView.cs
│       CoursesView.cs
│       DashboardView.cs
│       EnrollmentsView.cs
│       StudentsView.cs
│
├── Database/
│   └── schema.sql                       # Table definitions + sample seed data
│
├── CourseEnrollmentSystem.sln            # Visual Studio solution file
└── README.md
```

*Each form/view listed above has a matching `.resx` file alongside it, plus an auto-generated `.Designer.cs` partial class (regenerated automatically by the Visual Studio Forms Designer, containing only `InitializeComponent()` layout code) — both omitted here for readability.*

---

## Prerequisites

- Windows OS
- Visual Studio 2022 (v17.x or later) with the **.NET desktop development** workload installed
- .NET 10 SDK
- SQL Server (LocalDB, Express, or full instance)

---

## Database Setup

The application connects to a SQL Server database named `CourseEnrollmentDB`.

### Step 1 — Create the database

Run `Database/schema.sql` in SQL Server Management Studio (SSMS), Azure Data Studio, or any SQL client. It creates the database, all four tables below, and seeds a small set of sample data so the dashboard isn't empty on first run.

```sql
CREATE DATABASE CourseEnrollmentDB;
GO

USE CourseEnrollmentDB;
GO

CREATE TABLE Course (
    Id             NVARCHAR(20)  PRIMARY KEY,
    Code           NVARCHAR(20)  NULL,
    Title          NVARCHAR(200) NOT NULL,
    Instructor     NVARCHAR(150) NOT NULL,
    Department     NVARCHAR(50)  NOT NULL,
    Credits        INT           NOT NULL CHECK (Credits BETWEEN 1 AND 6),
    AvailableSeats INT           NOT NULL CHECK (AvailableSeats >= 0),
    Status         NVARCHAR(20)  NOT NULL DEFAULT 'Open'
);

CREATE TABLE Student (
    Id         NVARCHAR(20)  PRIMARY KEY,
    Name       NVARCHAR(150) NOT NULL,
    Phone      NVARCHAR(20)  NOT NULL,
    Email      NVARCHAR(150) NULL,
    Department NVARCHAR(100) NULL
);

CREATE TABLE Enrollment (
    Id          NVARCHAR(20)  PRIMARY KEY,
    CourseId    NVARCHAR(20)  NOT NULL REFERENCES Course(Id),
    CourseTitle NVARCHAR(200) NOT NULL,
    StudentId   NVARCHAR(20)  NOT NULL REFERENCES Student(Id),
    StudentName NVARCHAR(150) NOT NULL,
    EnrollDate  DATETIME      NOT NULL,
    EndDate     DATETIME      NULL,
    Grade       NVARCHAR(10)  NULL,
    Status      NVARCHAR(20)  NOT NULL DEFAULT 'Enrolled'
);

CREATE TABLE Announcement (
    Id         NVARCHAR(20)   PRIMARY KEY,
    Title      NVARCHAR(200)  NOT NULL,
    Message    NVARCHAR(1000) NOT NULL,
    PostedDate DATETIME       NOT NULL,
    IsActive   BIT            NOT NULL DEFAULT 1
);
```

### Step 2 — Configure the connection string

Open `CourseEnrollment.WindowsApp/App.config` and update the `Server` value to match your SQL Server instance:

```xml
<connectionStrings>
  <add name="CourseEnrollmentDB"
       connectionString="Server=localhost;Database=CourseEnrollmentDB;Trusted_Connection=True;TrustServerCertificate=True;"
       providerName="Microsoft.Data.SqlClient" />
</connectionStrings>
```

Common server name values:

| Instance type | Server value |
|---|---|
| SQL Server Express | `.\SQLEXPRESS` |
| LocalDB | `(localdb)\MSSQLLocalDB` |
| Default local instance | `localhost` or `.` |

---

## Getting Started

1. Clone the repository:
   ```
   git clone https://github.com/<your-username>/CourseEnrollmentSystem.git
   ```
2. Open `CourseEnrollmentSystem.sln` in Visual Studio.
3. Complete the [Database Setup](#database-setup) steps above.
4. Update the connection string in `CourseEnrollment.WindowsApp/App.config` if needed.
5. Right-click `CourseEnrollment.WindowsApp` in Solution Explorer → **Set as Startup Project**.
6. Press **F5** to build and run.

---

## License

This project is for educational use — COSC-5136 Advanced Programming, Spring 2026.
