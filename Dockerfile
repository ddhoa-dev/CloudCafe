# ===== STAGE 1: Build Frontend =====
FROM node:20-alpine AS frontend-build
WORKDIR /app/frontend
COPY frontend/package*.json ./
RUN npm install
COPY frontend/ ./
RUN npm run build

# ===== STAGE 2: Build Backend =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files và restore dependencies
COPY ["src/CafeManagement.API/CafeManagement.API.csproj", "src/CafeManagement.API/"]
COPY ["src/CafeManagement.Application/CafeManagement.Application.csproj", "src/CafeManagement.Application/"]
COPY ["src/CafeManagement.Infrastructure/CafeManagement.Infrastructure.csproj", "src/CafeManagement.Infrastructure/"]
COPY ["src/CafeManagement.Domain/CafeManagement.Domain.csproj", "src/CafeManagement.Domain/"]

RUN dotnet restore "src/CafeManagement.API/CafeManagement.API.csproj"

# Copy toàn bộ source code
COPY . .

# Build project
WORKDIR "/src/src/CafeManagement.API"
RUN dotnet build "CafeManagement.API.csproj" -c Release -o /app/build

# ===== STAGE 2: Publish =====
FROM build AS publish
RUN dotnet publish "CafeManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===== STAGE 3: Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Tạo non-root user để chạy app (security best practice)
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy published files từ stage publish
COPY --from=publish /app/publish .

# Copy frontend build sang thư mục wwwroot để .NET phục vụ dưới dụng static files
COPY --from=frontend-build /app/frontend/dist ./wwwroot

# Expose ports
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "CafeManagement.API.dll"]
