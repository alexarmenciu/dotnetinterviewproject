# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore as distinct layers
COPY . .

# Build and publish API
WORKDIR /src/API

# Build the project
RUN dotnet build -c Release


# Publish the API
RUN dotnet publish -c Release -o /app/api

# Build and publish Blazor frontend
WORKDIR /src/Frontend
# Install Node.js and npm for Tailwind build
RUN apt-get update && apt-get install -y curl && curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && apt-get install -y nodejs
# Install Tailwind CSS CLI
RUN npm install -g tailwindcss
# Build Tailwind CSS
RUN npx tailwindcss -i ./styles/tailwind.css -o ./wwwroot/styles.css
RUN dotnet publish -c Release -o /app/frontend

# Runtime image with both API and frontend
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy both applications
COPY --from=build /app/api ./api
COPY --from=build /app/frontend ./frontend


# Expose ports for both services
EXPOSE 5000 5001

# Set environment variable for frontend to find backend
ENV API_BASE_URL=http://localhost:5000

# Create startup script with proper port configuration
RUN echo '#!/bin/bash\n\
echo "Starting API on port 5000..."\n\
ASPNETCORE_URLS="http://+:5000" dotnet api/API.dll &\n\
API_PID=$!\n\
echo "Starting Frontend on port 5001..."\n\
cd /app/frontend\n\
ASPNETCORE_URLS="http://+:5001" dotnet Frontend.dll &\n\
FRONTEND_PID=$!\n\
wait $API_PID $FRONTEND_PID' > /app/start.sh && chmod +x /app/start.sh

ENTRYPOINT ["/app/start.sh"]

# Instructions:
# - To build and run: docker build -t dotnetapp . && docker run -p 5000:5000 -p 5001:5001 dotnetapp
# - Note: EF migrations are now applied during build time

