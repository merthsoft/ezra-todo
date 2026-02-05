# ToDoApp

A to-do list application built with .NET Aspire, ASP.NET Core, and React with TypeScript, using a retro theme to give it a terminal look.

## Overview

This application demonstrates a full-stack solution using:
- **Backend**: ASP.NET Core (.NET 10) API server
- **Database**: Sqlite using EFCore
- **Frontend**: React with TypeScript, built with Vite
- **Orchestration**: .NET Aspire for application hosting and service orchestration

## Assumptions

- Individual TODO items, no grouping etc.
- Single database, monolith
- Backend and frontend deployed from same server
- Various scalability assumptions (see `Scaling` below)
- Username is email
- Use JWT bearer for auth
- API only consumed by frontend--no external consumers

## Future

### Features

- Edit TODO items/"kick the can" functionality for due date--needs frontend support
- jwt secret in key vault (or similar)
- user email validation
- user settings for pagination, etc.
- user groups, tenants
- OAuth with seamless login from providers (Google, Facebook, Apple, etc.)
- Concurrent user access (think like how google docs does it)
- Daily/Weekly/Monthly TODO items that auto-fill
- Grouping, tagging, etc.
- Sorting on due date, status, tags, etc.
- Search
- Export (needs research for format)
- Better overdue indicators (for colorblind people)
- Shared items between users
- Shared lists between users
- Retry logic if server is unavailable
- Local caching in case internet goes down etc.
- Better date handling between client/server
- Favicon
- Duplicate task detection
- Consider API for external usage
- Frontend and integration tests

### Scaling

Currently, the application can handle a small/medium workload fairly well. However, there are some considerations for scaling:

- No pagination
- No query optimization
- No load balancing
- No caching
- No rate limiting

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [Node.js](https://nodejs.org/) (v20 or later) and npm
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (or later) or [Visual Studio Code](https://code.visualstudio.com/) with C# extension
- [.NET Aspire workload](https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling)

### Installing .NET Aspire Workload

```bash
dotnet workload install aspire
```

## Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd ToDoApp
```

### 2. Install Frontend Dependencies

Navigate to the frontend directory and install dependencies:

```bash
cd frontend
npm install
cd ..
```

### 3. Restore .NET Dependencies

```bash
dotnet restore
```

## Running the Application

### Option 1: Using .NET CLI

The easiest way to run the application is using the Aspire AppHost, which will automatically start both the backend and frontend:

```bash
dotnet run --project ToDoApp.AppHost
```

### Option 2: Using Visual Studio

1. Open the solution file (`.slnx`) in Visual Studio 2022 (or later)
2. Set `ToDoApp.AppHost` as the startup project
3. Press `F5` or click the "Start" button

## Usage Instructions

1. **Access the Application**
   - Once running, the Aspire Dashboard will open automatically
   - On first run, it may take some time for the web frontend to become available, you will see the status in the dashboard
   - Click on the `webfrontend` endpoint to open the application

2. **Logging In**
   - Register a new account
   - Alternatively, use `user@test.com` with password `Password123!` if using a dev deploy

3. **Using the To-Do App**
   - Add new to-do item using the input field
   - Set a due-date if you want
   - Mark items as complete/incomplete
   - Delete items as needed
   - Overdue items will be red

## Troubleshooting

### Certificate Issues

If you encounter HTTPS certificate errors:
```bash
dotnet dev-certs https --trust
```

### Port Conflicts

If ports are already in use, you can modify the configuration in:
- `ToDoApp.Server/Properties/launchSettings.json` (backend)
- `frontend/vite.config.ts` (frontend)

### Node Modules Issues

If you encounter dependency issues:
```bash
cd frontend
rm -rf node_modules package-lock.json
npm install
```
