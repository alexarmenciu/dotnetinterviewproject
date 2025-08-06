# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore as distinct layers
COPY . .

# Build and test API
WORKDIR /src/API
RUN dotnet build -c Release

# Run tests (fail build if tests fail) and save results
WORKDIR /src/API.Tests
RUN dotnet test --logger "trx;LogFileName=testResults.trx" --results-directory /app/testresults

# Publish the API
WORKDIR /src/API
RUN dotnet publish -c Release -o /app/api

# Build and publish Blazor frontend
WORKDIR /src/Frontend
# Install Node.js and npm for Tailwind build
RUN apt-get update && apt-get install -y curl && curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && apt-get install -y nodejs
# Install Tailwind CSS CLI
# Initialize npm and install Tailwind CSS locally
RUN npm init -y
RUN npm install tailwindcss
RUN npx tailwindcss -i ./styles/tailwind.css -o ./wwwroot/styles.css
RUN dotnet publish -c Release -o /app/frontend

# Runtime image with both API and frontend
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy both applications
COPY --from=build /app/api ./api
COPY --from=build /app/frontend ./frontend

# Copy test results to the final image
COPY --from=build /app/testresults ./testresults

# Create startup script with ports configurable via environment variables
RUN echo '#!/bin/bash\n\
API_PORT="${API_PORT:-5000}"\n\
FRONTEND_PORT="${FRONTEND_PORT:-5001}"\n\
export API_BASE_URL="http://localhost:$API_PORT"\n\
echo "Starting API on port $API_PORT..."\n\
ASPNETCORE_URLS="http://+:$API_PORT" dotnet api/API.dll &\n\
API_PID=$!\n\
echo "Starting Frontend on port $FRONTEND_PORT..."\n\
cd /app/frontend\n\
ASPNETCORE_URLS="http://+:$FRONTEND_PORT" dotnet Frontend.dll &\n\
FRONTEND_PID=$!\n\
wait $API_PID $FRONTEND_PID' > /app/start.sh && chmod +x /app/start.sh

# Set default environment variables
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/tasks.db"

# Create data directory for database
RUN mkdir -p /app/data

ENTRYPOINT ["/app/start.sh"]

# Instructions:
# - To build and run: docker build -t dotnetinterviewproject . && docker run -p 5000:5000 -p 5001:5001 dotnetinterviewproject
# - To use custom database: docker run -p 5000:5000 -p 5001:5001 -e "ConnectionStrings__DefaultConnection=Data Source=/app/data/custom.db" dotnetinterviewproject
# - To use SQL Server: docker run -p 5000:5000 -p 5001:5001 -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=TasksDB;Trusted_Connection=true;" dotnetinterviewproject
# - To persist data: docker run -p 5000:5000 -p 5001:5001 -v /path/to/data:/app/data dotnetinterviewproject
# - Access test results inside container: docker exec -it <container_id> ls /app/testresults
# - Copy test results to host: docker cp <container_id>:/app/testresults ./testresults
# - Note: EF migrations are now applied during build time