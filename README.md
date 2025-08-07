# Dotnet Interview Project

Overview: All requirements, plus the additional nice to have features, have been implemented in this project. The README file describes how to run the project, including database migrations and Docker setup.

## Running Database Migrations

You can apply pending Entity Framework Core migrations to your database using the following commands from the `/API/` folder (same for both Windows and Linux/macOS). The project includes an initial migration FirstCreate.

```sh
cd API
dotnet ef database update --connection "YourConnectionString"
```

These migrations will also populate the database with initial data.

## Dockerizing the Application

### 1. Build the Docker image

```sh
docker build -t dotnetinterviewproject .
```

### 2. Run the Docker container

```sh
docker run -e API_PORT=5000 -e FRONTEND_PORT=5001 -p 5000:5000 -p 5001:5001 dotnetinterviewproject
```

- This will expose the API on port 5000 and the Blazor frontend on port 5001.
- The container will automatically start both services with proper port configuration.

### 3. Access your application

- Frontend: [http://localhost:5001/tasks](http://localhost:5001/tasks)

- API Spec: [http://localhost:5000/swagger](http://localhost:5000/swagger)
- API: [http://localhost:5000/tasks](http://localhost:5000/tasks)

### 4. Running Tests

- Tests are run during the Docker build process.
- The test results will be available in the `/app/testResults.trx` file inside the container.

### 5. Custom Database Connection

You can specify a custom database connection string using environment variables:

**SQLite (default):**

```sh
docker run -p 5000:5000 -p 5001:5001 \
  -e "ConnectionStrings__DefaultConnection=Data Source=/app/data/myapp.db" \
  dotnetinterviewproject
```

**SQL Server:**

```sh
docker run -p 5000:5000 -p 5001:5001 \
  -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=TasksDB;User Id=sa;Password=YourPassword;" \
  -e "DatabaseProvider=SqlServer" \
  dotnetinterviewproject
```

---

## Running the Application Without Docker

### 1. Running the API

You can set the API port using the `API_PORT` environment variable.

**Windows (PowerShell):**

```powershell
$env:API_PORT=5000
cd API
dotnet build -c Release
dotnet run --urls=http://localhost:$env:API_PORT
```

**Linux/macOS (bash):**

```sh
export API_PORT=5000
cd API
dotnet build -c Release
dotnet run --urls=http://localhost:$API_PORT
```

The API will be available at [http://localhost:5000](http://localhost:5000) (or your chosen port).

### 2. Running the Frontend

You can set the frontend port using the `FRONTEND_PORT` environment variable.

**Windows (PowerShell):**

```powershell
$env:FRONTEND_PORT=5001
cd Frontend
# Install npm dependencies (if not already installed)
npm install
# Build Tailwind CSS
npx tailwindcss -i ./styles/tailwind.css -o ./wwwroot/styles.css
# Run the Blazor frontend
dotnet run --urls=http://localhost:$env:FRONTEND_PORT
```

**Linux/macOS (bash):**

```sh
export FRONTEND_PORT=5001
cd Frontend
# Install npm dependencies (if not already installed)
npm install
# Build Tailwind CSS
npx tailwindcss -i ./styles/tailwind.css -o ./wwwroot/styles.css
# Run the Blazor frontend
dotnet run --urls=http://localhost:$FRONTEND_PORT
```

The frontend will be available at [http://localhost:5001/tasks](http://localhost:5001/tasks) (or your chosen port).

### 3. Running Tests

To run the tests and view results:

```sh
cd API.Tests
dotnet test --logger "trx;LogFileName=testResults.trx" --results-directory ./testresults
```

Test results will be saved in the `API.Tests/testresults` directory.

---

## Validation Assumptions

The application uses FluentValidation to enforce business rules on the `Task` model. The following assumptions are made in the validators:

- **Title**

  - Must not be empty.
  - Must be between 1 and 200 characters.

- **Description**

  - Must not be empty.
  - Must be between 1 and 1000 characters.
  - Cannot be only whitespace.

- **Due Date**
  - Must not be empty.
  - Must be either today or in the future.
