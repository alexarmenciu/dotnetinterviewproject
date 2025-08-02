# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore as distinct layers
COPY . .

# Build and publish API
WORKDIR /src/API
RUN dotnet publish -c Release -o /app/api

# Build and publish Blazor frontend
WORKDIR /src/Frontend
RUN dotnet publish -c Release -o /app/frontend

# Runtime image with both API and frontend
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy both applications
COPY --from=build /app/api ./api
COPY --from=build /app/frontend ./frontend

# Expose ports for both services
EXPOSE 5000 5001

# Create startup script with proper port configuration
RUN echo '#!/bin/bash\n\
echo "Starting API on port 5000..."\n\
ASPNETCORE_URLS="http://+:5000" dotnet api/API.dll &\n\
API_PID=$!\n\
echo "Starting Frontend on port 5001..."\n\
ASPNETCORE_URLS="http://+:5001" dotnet frontend/Frontend.dll &\n\
FRONTEND_PID=$!\n\
wait $API_PID $FRONTEND_PID' > /app/start.sh && chmod +x /app/start.sh

ENTRYPOINT ["/app/start.sh"]

# Instructions:
# - To build and run: docker build -t dotnetapp . && docker run -p 5000:5000 -p 5001:5001 dotnetapp
# - Note: EF migrations should be run separately or as part of application startup

# Copy both applications
COPY --from=build /app/api ./api
COPY --from=build /app/frontend ./frontend

# Expose ports for both services
EXPOSE 5000 5001

# Create startup script
RUN echo '#!/bin/bash\n\
echo "Starting API..."\n\
dotnet api/API.dll &\n\
echo "Starting Frontend..."\n\
dotnet frontend/Frontend.dll &\n\
wait' > /app/start.sh && chmod +x /app/start.sh

ENTRYPOINT ["/app/start.sh"]

# Instructions:
# - To run the API: docker build --target api -t myapi . && docker run -p 5000:5000 myapi
# - To run the Blazor frontend: docker build --target blazor -t myblazor . && docker run -p 8080:80 myblazor

