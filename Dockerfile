# Dockerfile for containerizing the application
# Use an official base image (e.g., node:20, python:3.12, etc.)
FROM ubuntu:24.04

# Install dependencies (customize as needed)
RUN apt-get update && apt-get install -y \
    # Add your dependencies here, e.g. nodejs, python3, etc.
    && rm -rf /var/lib/apt/lists/*

# Set work directory
WORKDIR /app

# Copy application files
COPY . /app


# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore as distinct layers
COPY . .

# Build and publish API
WORKDIR /src/api
RUN dotnet publish -c Release -o /app/api

# Build and publish Blazor frontend
WORKDIR /src/frontend
RUN dotnet publish -c Release -o /app/blazor

# Runtime image for API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS api
WORKDIR /app
COPY --from=build /app/api .
EXPOSE 5000
# To apply EF Core migrations at container startup, add a script or entrypoint to run:
# dotnet ef database update
# (You may need to install dotnet-ef tools and set connection strings)
ENTRYPOINT ["dotnet", "api.dll"]

# Runtime image for Blazor WASM (static files)
FROM nginx:alpine AS blazor
COPY --from=build /app/blazor/wwwroot /usr/share/nginx/html
EXPOSE 80

# Instructions:
# - To run the API: docker build --target api -t myapi . && docker run -p 5000:5000 myapi
# - To run the Blazor frontend: docker build --target blazor -t myblazor . && docker run -p 8080:80 myblazor
# - For EF Core migrations, run them at container startup or manually before running the API container.

