# Smart Road Monitor / Accident Detection System

This project is a full-stack road monitoring system with:

- React + Vite frontend
- ASP.NET Core Web API backend
- SQLite database
- Optional FastAPI AI analysis backend for image upload/prediction

Use this guide if you are cloning the project on a new computer and nothing is installed yet.

## Project Structure

```text
AccidentDetectionSysrem_v2/
  src/             React frontend source code
  api/             ASP.NET Core backend API
  ai-backend/      FastAPI AI analysis service
  package.json     Frontend dependencies and scripts
  NuGet.Config     NuGet package source config
```

## Required Software

Install these first:

1. **Git**
   - Download: https://git-scm.com/downloads

2. **Node.js**
   - Download the LTS version: https://nodejs.org/
   - After installing, check:

   ```powershell
   node --version
   npm --version
   ```

3. **.NET SDK**
   - This project targets `.NET 10.0`.
   - Download: https://dotnet.microsoft.com/download
   - Check:

   ```powershell
   dotnet --version
   ```

4. **Python**
   - Download: https://www.python.org/downloads/
   - During install, enable **Add Python to PATH**.
   - Check:

   ```powershell
   python --version
   pip --version
   ```

## Clone the Project

```powershell
git clone <REPO_URL>
cd AccidentDetectionSysrem_v2
```

Replace `<REPO_URL>` with the GitHub repo link.

## Frontend Setup

Install frontend packages:

```powershell
npm install
```

Create a `.env` file in the project root:

```env
VITE_API_BASE_URL=http://localhost:5070/api
VITE_AI_API_BASE_URL=http://localhost:8000
```

If you want to use the deployed AI service instead of local FastAPI, set:

```env
VITE_AI_API_BASE_URL=https://alimoking2003-smart-system-for-road-monitoring.hf.space
```

## .NET Backend Setup

Go to the backend folder:

```powershell
cd api
dotnet restore
dotnet build
```

The backend uses SQLite. The database file is:

```text
api/SmartAccidentdb.db
```

If the database file is missing or you need to recreate it:

```powershell
dotnet ef database update
```

If `dotnet ef` is not installed:

```powershell
dotnet tool install --global dotnet-ef
```

Then return to the project root:

```powershell
cd ..
```

## FastAPI AI Backend Setup

This service is used by the upload / AI analysis page.

Create a Python virtual environment:

```powershell
cd ai-backend
python -m venv .venv
```

Activate it:

```powershell
.venv\Scripts\activate
```

Install packages:

```powershell
pip install -r requirements.txt
```

Return to the project root:

```powershell
cd ..
```

## Run the System

You need three terminals.

### Terminal 1: Run .NET API

From the project root:

```powershell
dotnet run --project api\AccidentDetectionSysrem.csproj --launch-profile http
```

Backend URL:

```text
http://localhost:5070
```

Swagger API docs:

```text
http://localhost:5070/swagger/index.html
```

### Terminal 2: Run React Frontend

From the project root:

```powershell
npm run dev
```

Frontend URL:

```text
http://127.0.0.1:5173/
```

### Terminal 3: Run FastAPI AI Backend

From the project root:

```powershell
cd ai-backend
.venv\Scripts\activate
python -m uvicorn main:app --host 127.0.0.1 --port 8000 --reload
```

FastAPI health check:

```text
http://localhost:8000/health
```

## Quick Test

Open these URLs:

```text
Frontend:     http://127.0.0.1:5173/
.NET API:     http://localhost:5070/swagger/index.html
AI Backend:   http://localhost:8000/health
```

Expected result:

- Frontend page opens.
- Swagger page opens.
- AI health endpoint returns:

```json
{"status":"healthy"}
```

## User Accounts

You can create a new account from:

```text
http://127.0.0.1:5173/signup
```

Available roles:

- `Admin`
- `Officer`
- `User`

Role redirects:

- `Admin` -> User Management
- `Officer` -> Weekly Report
- `User` -> Main Dashboard

## Useful Commands

Build frontend:

```powershell
npm run build
```

Run frontend preview after build:

```powershell
npm run preview
```

Build .NET backend:

```powershell
dotnet build api\AccidentDetectionSysrem.csproj
```

Check running ports:

```powershell
netstat -ano | findstr "5070 5173 8000"
```

## Common Problems and Fixes

### Network error on sign up or sign in

Make sure the .NET backend is running:

```text
http://localhost:5070/swagger/index.html
```

Also make sure `.env` contains:

```env
VITE_API_BASE_URL=http://localhost:5070/api
```

After editing `.env`, restart the frontend server.

### Upload / AI analysis does not work

Make sure FastAPI is running:

```text
http://localhost:8000/health
```

Also make sure `.env` contains:

```env
VITE_AI_API_BASE_URL=http://localhost:8000
```

### Port already in use

Check which process uses the port:

```powershell
netstat -ano | findstr "5070"
netstat -ano | findstr "5173"
netstat -ano | findstr "8000"
```

Stop the process from Task Manager, or change the port in the run command.

### `npm install` fails

Try clearing the npm cache:

```powershell
npm cache clean --force
npm install
```

### `dotnet restore` fails

Make sure the .NET SDK is installed and the project root contains `NuGet.Config`.

Then run:

```powershell
dotnet restore api\AccidentDetectionSysrem.csproj
```

### `python -m uvicorn` says module not found

Activate the virtual environment and install requirements again:

```powershell
cd ai-backend
.venv\Scripts\activate
pip install -r requirements.txt
python -m uvicorn main:app --host 127.0.0.1 --port 8000 --reload
```

## Recommended Startup Order

1. Start `.NET API`.
2. Start `FastAPI AI backend`.
3. Start `React frontend`.
4. Open `http://127.0.0.1:5173/`.

## Notes for Developers

- Frontend API client: `src/services/api.js`
- AI analysis service: `src/services/aiAnalysis.service.js`
- Authentication controller: `api/Controllers/AuthenticationController.cs`
- Main backend config: `api/appsettings.json`
- FastAPI entry point: `ai-backend/main.py`
