# Hướng dẫn Deployment

## GitHub Actions CI/CD

Dự án đã được cấu hình với 2 workflows:

### 1. CI/CD Pipeline (.github/workflows/ci-cd.yml)

Tự động chạy khi:
- Push code lên branch `main` hoặc `develop`
- Tạo Pull Request vào `main` hoặc `develop`

**Các bước:**
1. **Code Quality Check**: Kiểm tra format code
2. **Build & Test**: Build solution và chạy unit tests
3. **Build Docker Image**: Build và push Docker image lên GitHub Container Registry
4. **Security Scan**: Scan vulnerabilities với Trivy

### 2. Deploy Pipeline (.github/workflows/deploy.yml)

Tự động chạy khi:
- Publish release mới
- Trigger manual từ GitHub Actions UI

**Các bước:**
1. Build và publish application
2. Deploy lên server qua SSH
3. Restart application service
4. Health check

## Setup GitHub Secrets

Vào **Settings → Secrets and variables → Actions** và thêm:

### Cho CI/CD Pipeline:
- `GITHUB_TOKEN`: Tự động có sẵn (không cần thêm)

### Cho Deploy Pipeline:
- `SERVER_HOST`: IP hoặc domain của server
- `SERVER_USERNAME`: SSH username
- `SERVER_SSH_KEY`: Private SSH key
- `APP_URL`: URL của application (để health check)

## Deploy lên Server (Manual)

### Yêu cầu Server:
- Ubuntu 20.04+ hoặc Debian 11+
- .NET 8 Runtime
- PostgreSQL 16
- Nginx (reverse proxy)

### Bước 1: Cài đặt .NET Runtime

```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0 --runtime aspnetcore
```

### Bước 2: Cài đặt PostgreSQL

```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql

# Tạo database
sudo -u postgres psql
CREATE DATABASE CafeManagementDb;
CREATE USER cafeuser WITH PASSWORD 'your_password';
GRANT ALL PRIVILEGES ON DATABASE CafeManagementDb TO cafeuser;
\q
```

### Bước 3: Deploy Application

```bash
# Clone repository
git clone <repository-url>
cd CafeManagement

# Build và publish
dotnet publish src/CafeManagement.API/CafeManagement.API.csproj -c Release -o /var/www/cafemanagement

# Tạo appsettings.Production.json
sudo nano /var/www/cafemanagement/appsettings.Production.json
```

**appsettings.Production.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CafeManagementDb;Username=cafeuser;Password=your_password"
  },
  "JwtSettings": {
    "Secret": "production-secret-key-must-be-at-least-32-characters-long-and-secure",
    "Issuer": "CafeManagementAPI",
    "Audience": "CafeManagementClient",
    "ExpiryMinutes": 60
  }
}
```

### Bước 4: Tạo Systemd Service

```bash
sudo nano /etc/systemd/system/cafemanagement.service
```

**cafemanagement.service:**
```ini
[Unit]
Description=Cafe Management API
After=network.target

[Service]
Type=notify
WorkingDirectory=/var/www/cafemanagement
ExecStart=/usr/bin/dotnet /var/www/cafemanagement/CafeManagement.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=cafemanagement
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

```bash
# Enable và start service
sudo systemctl daemon-reload
sudo systemctl enable cafemanagement
sudo systemctl start cafemanagement
sudo systemctl status cafemanagement
```

### Bước 5: Chạy Migrations

```bash
cd /var/www/cafemanagement
dotnet ef database update --project CafeManagement.Infrastructure.dll --startup-project CafeManagement.API.dll
```

### Bước 6: Cấu hình Nginx

```bash
sudo nano /etc/nginx/sites-available/cafemanagement
```

**cafemanagement:**
```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
# Enable site
sudo ln -s /etc/nginx/sites-available/cafemanagement /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

### Bước 7: SSL với Let's Encrypt (Optional)

```bash
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d your-domain.com
```

## Deploy với Docker

### Bước 1: Build Image

```bash
docker build -t cafemanagement-api:latest .
```

### Bước 2: Run với Docker Compose

```bash
docker-compose up -d
```

### Bước 3: Chạy Migrations

```bash
docker exec -it cafe-api dotnet ef database update --project /src/src/CafeManagement.Infrastructure --startup-project /src/src/CafeManagement.API
```

## Monitoring & Logs

### Xem logs từ Systemd

```bash
sudo journalctl -u cafemanagement -f
```

### Xem logs từ Docker

```bash
docker-compose logs -f api
```

### Xem logs từ file

```bash
tail -f /var/www/cafemanagement/logs/log-*.txt
```

## Backup Database

### Manual Backup

```bash
pg_dump -U cafeuser -h localhost CafeManagementDb > backup_$(date +%Y%m%d_%H%M%S).sql
```

### Automated Backup (Cron)

```bash
crontab -e
```

Thêm dòng:
```
0 2 * * * pg_dump -U cafeuser -h localhost CafeManagementDb > /backups/cafe_$(date +\%Y\%m\%d).sql
```

## Rollback

### Rollback với Git

```bash
cd /var/www/cafemanagement
git checkout <previous-commit>
dotnet publish -c Release -o .
sudo systemctl restart cafemanagement
```

### Rollback Database

```bash
psql -U cafeuser -h localhost CafeManagementDb < backup_20260406_020000.sql
```

## Health Check

```bash
curl http://localhost:5000/health
```

## Troubleshooting

### Service không start
```bash
sudo journalctl -u cafemanagement -n 50
```

### Database connection failed
- Kiểm tra PostgreSQL đang chạy: `sudo systemctl status postgresql`
- Kiểm tra connection string trong appsettings.Production.json
- Test connection: `psql -U cafeuser -h localhost CafeManagementDb`

### Port already in use
```bash
sudo lsof -i :5000
sudo kill -9 <PID>
```
