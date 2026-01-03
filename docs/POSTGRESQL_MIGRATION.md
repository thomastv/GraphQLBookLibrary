# Migrating to PostgreSQL with Docker

This guide provides comprehensive steps to migrate the GraphQL Book Library from SQLite to PostgreSQL running in a Docker container.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Docker Setup](#docker-setup)
- [PostgreSQL Container Configuration](#postgresql-container-configuration)
- [Networking](#networking)
- [Data Persistence with Volumes](#data-persistence-with-volumes)
- [Code Changes Required](#code-changes-required)
- [Database Migration](#database-migration)
- [Data Migration](#data-migration)
- [Docker Compose Setup](#docker-compose-setup)
- [Production Considerations](#production-considerations)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Basic familiarity with Docker commands

### Verify Docker Installation

```bash
docker --version
docker compose version
```

---

## Docker Setup

### Option 1: Simple Docker Run Command

Quick start for development:

```bash
docker run -d \
  --name booklibrary-postgres \
  -e POSTGRES_USER=booklibrary \
  -e POSTGRES_PASSWORD=YourSecurePassword123! \
  -e POSTGRES_DB=booklibrary \
  -p 5432:5432 \
  postgres:16-alpine
```

**Windows PowerShell:**

```powershell
docker run -d `
  --name booklibrary-postgres `
  -e POSTGRES_USER=booklibrary `
  -e POSTGRES_PASSWORD=YourSecurePassword123! `
  -e POSTGRES_DB=booklibrary `
  -p 5432:5432 `
  postgres:16-alpine
```

### Verify Container is Running

```bash
docker ps
docker logs booklibrary-postgres
```

### Connect to PostgreSQL

```bash
docker exec -it booklibrary-postgres psql -U booklibrary -d booklibrary
```

Once connected, you can run SQL commands:

```sql
\dt                    -- List tables
\d+ "Books"           -- Describe Books table
SELECT * FROM "Authors";
\q                    -- Quit
```

---

## PostgreSQL Container Configuration

### Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `POSTGRES_USER` | Database superuser name | `booklibrary` |
| `POSTGRES_PASSWORD` | Superuser password | `YourSecurePassword123!` |
| `POSTGRES_DB` | Default database name | `booklibrary` |
| `PGDATA` | Data directory inside container | `/var/lib/postgresql/data/pgdata` |

### Port Mapping

| Host Port | Container Port | Protocol |
|-----------|----------------|----------|
| 5432 | 5432 | TCP |

If port 5432 is already in use, map to a different host port:

```bash
docker run -d -p 5433:5432 ...  # Access via localhost:5433
```

### Resource Limits (Optional)

```bash
docker run -d \
  --name booklibrary-postgres \
  --memory=512m \
  --cpus=1.0 \
  -e POSTGRES_USER=booklibrary \
  -e POSTGRES_PASSWORD=YourSecurePassword123! \
  -e POSTGRES_DB=booklibrary \
  -p 5432:5432 \
  postgres:16-alpine
```

---

## Networking

### Default Bridge Network

By default, Docker containers run on the bridge network. The API can connect using:
- **From host machine**: `localhost:5432` or `127.0.0.1:5432`
- **From another container**: `container_name:5432` or `container_ip:5432`

### Create Custom Network (Recommended)

Custom networks provide DNS resolution between containers:

```bash
# Create network
docker network create booklibrary-network

# Run PostgreSQL on custom network
docker run -d \
  --name booklibrary-postgres \
  --network booklibrary-network \
  -e POSTGRES_USER=booklibrary \
  -e POSTGRES_PASSWORD=YourSecurePassword123! \
  -e POSTGRES_DB=booklibrary \
  -p 5432:5432 \
  postgres:16-alpine
```

### Network Commands

```bash
# List networks
docker network ls

# Inspect network
docker network inspect booklibrary-network

# Connect existing container to network
docker network connect booklibrary-network container_name

# Disconnect from network
docker network disconnect booklibrary-network container_name
```

### Connection from API

| Scenario | Host | Port |
|----------|------|------|
| API running on host machine | `localhost` | `5432` |
| API in container (same network) | `booklibrary-postgres` | `5432` |
| API in container (different network) | Container IP | `5432` |

---

## Data Persistence with Volumes

**Important**: Without volumes, data is lost when the container is removed!

### Option 1: Named Volume (Recommended)

Docker manages the volume location:

```bash
# Create named volume
docker volume create booklibrary-pgdata

# Run with named volume
docker run -d \
  --name booklibrary-postgres \
  -e POSTGRES_USER=booklibrary \
  -e POSTGRES_PASSWORD=YourSecurePassword123! \
  -e POSTGRES_DB=booklibrary \
  -v booklibrary-pgdata:/var/lib/postgresql/data \
  -p 5432:5432 \
  postgres:16-alpine
```

### Option 2: Bind Mount

Map to a specific host directory:

**Linux/macOS:**

```bash
docker run -d \
  --name booklibrary-postgres \
  -e POSTGRES_USER=booklibrary \
  -e POSTGRES_PASSWORD=YourSecurePassword123! \
  -e POSTGRES_DB=booklibrary \
  -v /path/to/data:/var/lib/postgresql/data \
  -p 5432:5432 \
  postgres:16-alpine
```

**Windows PowerShell:**

```powershell
docker run -d `
  --name booklibrary-postgres `
  -e POSTGRES_USER=booklibrary `
  -e POSTGRES_PASSWORD=YourSecurePassword123! `
  -e POSTGRES_DB=booklibrary `
  -v C:\docker\postgres\data:/var/lib/postgresql/data `
  -p 5432:5432 `
  postgres:16-alpine
```

### Volume Commands

```bash
# List volumes
docker volume ls

# Inspect volume
docker volume inspect booklibrary-pgdata

# Remove volume (DELETES ALL DATA!)
docker volume rm booklibrary-pgdata

# Remove unused volumes
docker volume prune
```

### Backup and Restore

**Backup:**

```bash
docker exec -t booklibrary-postgres pg_dump -U booklibrary booklibrary > backup.sql
```

**Restore:**

```bash
docker exec -i booklibrary-postgres psql -U booklibrary booklibrary < backup.sql
```

---

## Code Changes Required

### 1. Add NuGet Package

Add PostgreSQL provider to `BookLibrary.Api`:

```bash
cd src/BookLibrary.Api
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

Or add to `Directory.Packages.props`:

```xml
<PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
```

Then reference in `BookLibrary.Api.csproj`:

```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
```

### 2. Update Connection String

**appsettings.json:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=booklibrary;Username=booklibrary;Password=YourSecurePassword123!"
  }
}
```

**appsettings.Development.json:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=booklibrary;Username=booklibrary;Password=YourSecurePassword123!"
  }
}
```

### 3. Update Program.cs

Change from SQLite to PostgreSQL:

```csharp
// Before (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// After (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### 4. Update AppDbContext (if needed)

PostgreSQL has different conventions. You may need to configure:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // PostgreSQL uses lowercase table names by default
    // To keep PascalCase, configure explicitly:
    modelBuilder.Entity<Author>().ToTable("Authors");
    modelBuilder.Entity<Book>().ToTable("Books");
    modelBuilder.Entity<Genre>().ToTable("Genres");
    modelBuilder.Entity<Review>().ToTable("Reviews");
    
    // ... existing configuration
}
```

### 5. Handle PostgreSQL-Specific Types (Optional)

PostgreSQL supports additional types:

```csharp
// Use PostgreSQL array type
public string[] Tags { get; set; }

// Use PostgreSQL JSON type
[Column(TypeName = "jsonb")]
public string Metadata { get; set; }
```

---

## Database Migration

### Delete Existing SQLite Migrations

```bash
# Windows
Remove-Item -Recurse -Force src\BookLibrary.Api\Migrations\

# Linux/macOS
rm -rf src/BookLibrary.Api/Migrations/
```

### Create New PostgreSQL Migration

```bash
cd src/BookLibrary.Api

# Ensure PostgreSQL container is running
docker start booklibrary-postgres

# Create migration
dotnet ef migrations add InitialPostgreSQL

# Apply migration
dotnet ef database update
```

### Verify Migration

```bash
docker exec -it booklibrary-postgres psql -U booklibrary -d booklibrary -c "\dt"
```

Expected output:

```
          List of relations
 Schema |   Name    | Type  |    Owner    
--------+-----------+-------+-------------
 public | Authors   | table | booklibrary
 public | Books     | table | booklibrary
 public | Genres    | table | booklibrary
 public | Reviews   | table | booklibrary
 public | BookGenre | table | booklibrary
```

---

## Data Migration

### Option 1: Let Seed Data Recreate

Simplest approach - just run the application:

```bash
dotnet run --project src/BookLibrary.Api
```

The `SeedData.InitializeAsync()` will populate the database.

### Option 2: Export from SQLite, Import to PostgreSQL

**Step 1: Export from SQLite**

```bash
sqlite3 src/BookLibrary.Api/booklibrary.db

.mode csv
.headers on
.output authors.csv
SELECT * FROM Authors;
.output books.csv
SELECT * FROM Books;
.output genres.csv
SELECT * FROM Genres;
.output reviews.csv
SELECT * FROM Reviews;
.output bookgenre.csv
SELECT * FROM BookGenre;
.quit
```

**Step 2: Import to PostgreSQL**

```bash
# Copy CSV files to container
docker cp authors.csv booklibrary-postgres:/tmp/
docker cp books.csv booklibrary-postgres:/tmp/
docker cp genres.csv booklibrary-postgres:/tmp/
docker cp reviews.csv booklibrary-postgres:/tmp/
docker cp bookgenre.csv booklibrary-postgres:/tmp/

# Import data
docker exec -it booklibrary-postgres psql -U booklibrary -d booklibrary

\copy "Authors" FROM '/tmp/authors.csv' WITH CSV HEADER;
\copy "Genres" FROM '/tmp/genres.csv' WITH CSV HEADER;
\copy "Books" FROM '/tmp/books.csv' WITH CSV HEADER;
\copy "Reviews" FROM '/tmp/reviews.csv' WITH CSV HEADER;
\copy "BookGenre" FROM '/tmp/bookgenre.csv' WITH CSV HEADER;
```

### Option 3: Use pgloader (Advanced)

[pgloader](https://pgloader.io/) can migrate directly from SQLite to PostgreSQL:

```bash
# Install pgloader
docker pull dimitri/pgloader

# Run migration
docker run --rm \
  -v $(pwd)/src/BookLibrary.Api:/data \
  --network host \
  dimitri/pgloader \
  pgloader /data/booklibrary.db postgresql://booklibrary:YourSecurePassword123!@localhost/booklibrary
```

---

## Docker Compose Setup

Create `docker-compose.yml` in the project root for easier management:

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    container_name: booklibrary-postgres
    restart: unless-stopped
    environment:
      POSTGRES_USER: booklibrary
      POSTGRES_PASSWORD: YourSecurePassword123!
      POSTGRES_DB: booklibrary
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data
      - ./init-scripts:/docker-entrypoint-initdb.d  # Optional: initialization scripts
    networks:
      - booklibrary-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U booklibrary -d booklibrary"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Optional: pgAdmin for database management
  pgadmin:
    image: dpage/pgadmin4:latest
    container_name: booklibrary-pgadmin
    restart: unless-stopped
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@booklibrary.local
      PGADMIN_DEFAULT_PASSWORD: admin
    ports:
      - "5050:80"
    volumes:
      - pgadmin-data:/var/lib/pgadmin
    networks:
      - booklibrary-network
    depends_on:
      - postgres

volumes:
  postgres-data:
    name: booklibrary-pgdata
  pgadmin-data:
    name: booklibrary-pgadmin

networks:
  booklibrary-network:
    name: booklibrary-network
    driver: bridge
```

### Docker Compose Commands

```bash
# Start services
docker compose up -d

# View logs
docker compose logs -f postgres

# Stop services
docker compose stop

# Stop and remove containers
docker compose down

# Stop, remove containers AND volumes (DELETES DATA!)
docker compose down -v

# Rebuild and start
docker compose up -d --build
```

### Access pgAdmin

1. Open http://localhost:5050
2. Login with `admin@booklibrary.local` / `admin`
3. Add server:
   - Name: `BookLibrary`
   - Host: `postgres` (Docker service name)
   - Port: `5432`
   - Username: `booklibrary`
   - Password: `YourSecurePassword123!`

---

## Production Considerations

### 1. Use Secrets for Passwords

**Docker Swarm secrets:**

```yaml
services:
  postgres:
    secrets:
      - postgres_password
    environment:
      POSTGRES_PASSWORD_FILE: /run/secrets/postgres_password

secrets:
  postgres_password:
    file: ./secrets/postgres_password.txt
```

**.NET User Secrets (Development):**

```bash
cd src/BookLibrary.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;..."
```

**Environment Variables (Production):**

```bash
export ConnectionStrings__DefaultConnection="Host=..."
```

### 2. Connection Pooling

PostgreSQL benefits from connection pooling:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=booklibrary;Username=booklibrary;Password=YourSecurePassword123!;Pooling=true;MinPoolSize=5;MaxPoolSize=100"
  }
}
```

### 3. SSL/TLS Connections

For production, enable SSL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=...;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

### 4. Backup Strategy

**Automated daily backup:**

```bash
#!/bin/bash
# backup.sh
BACKUP_DIR=/backups
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

docker exec booklibrary-postgres pg_dump -U booklibrary booklibrary | gzip > $BACKUP_DIR/booklibrary_$TIMESTAMP.sql.gz

# Keep only last 7 days
find $BACKUP_DIR -name "*.sql.gz" -mtime +7 -delete
```

### 5. Monitoring

Add health check endpoint:

```yaml
healthcheck:
  test: ["CMD-SHELL", "pg_isready -U booklibrary -d booklibrary"]
  interval: 30s
  timeout: 10s
  retries: 3
  start_period: 30s
```

### 6. Performance Tuning

PostgreSQL configuration via environment or config file:

```yaml
services:
  postgres:
    command:
      - "postgres"
      - "-c"
      - "max_connections=200"
      - "-c"
      - "shared_buffers=256MB"
      - "-c"
      - "effective_cache_size=768MB"
      - "-c"
      - "maintenance_work_mem=64MB"
      - "-c"
      - "checkpoint_completion_target=0.9"
      - "-c"
      - "wal_buffers=16MB"
      - "-c"
      - "default_statistics_target=100"
```

---

## Troubleshooting

### Container Won't Start

```bash
# Check logs
docker logs booklibrary-postgres

# Common issues:
# - Port already in use: change host port
# - Volume permissions: check directory permissions
# - Invalid password: must meet complexity requirements
```

### Connection Refused

1. Verify container is running: `docker ps`
2. Check port mapping: `docker port booklibrary-postgres`
3. Test connection: `docker exec -it booklibrary-postgres pg_isready`
4. Check firewall settings

### Authentication Failed

```bash
# Reset password
docker exec -it booklibrary-postgres psql -U postgres -c "ALTER USER booklibrary PASSWORD 'NewPassword';"
```

### Database Does Not Exist

```bash
docker exec -it booklibrary-postgres psql -U postgres -c "CREATE DATABASE booklibrary OWNER booklibrary;"
```

### Permission Denied on Volume

**Windows:** Ensure Docker has access to the drive in Docker Desktop settings.

**Linux:**

```bash
# Check ownership
ls -la /path/to/data

# Fix permissions
sudo chown -R 999:999 /path/to/data  # 999 is postgres user in container
```

### Migration Errors

```bash
# Drop and recreate database
docker exec -it booklibrary-postgres psql -U postgres -c "DROP DATABASE booklibrary;"
docker exec -it booklibrary-postgres psql -U postgres -c "CREATE DATABASE booklibrary OWNER booklibrary;"

# Re-run migration
dotnet ef database update
```

### Check PostgreSQL Version Compatibility

```bash
# In container
docker exec -it booklibrary-postgres psql -U booklibrary -c "SELECT version();"

# EF Core Npgsql compatibility:
# - Npgsql 9.x supports PostgreSQL 12-17
# - Npgsql 8.x supports PostgreSQL 11-16
```

---

## Quick Reference

### Connection String Format

```
Host=hostname;Port=5432;Database=dbname;Username=user;Password=pass
```

### Common Docker Commands

```bash
# Start container
docker start booklibrary-postgres

# Stop container
docker stop booklibrary-postgres

# Remove container
docker rm booklibrary-postgres

# Shell into container
docker exec -it booklibrary-postgres bash

# PostgreSQL CLI
docker exec -it booklibrary-postgres psql -U booklibrary -d booklibrary
```

### Common psql Commands

```sql
\l              -- List databases
\c dbname       -- Connect to database
\dt             -- List tables
\d+ tablename   -- Describe table
\du             -- List users
\q              -- Quit
```
