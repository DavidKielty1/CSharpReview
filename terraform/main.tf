terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "~> 3.0"
    }
  }
}

provider "docker" {}

# Create a network for our services
resource "docker_network" "geekmeet_network" {
  name = "geekmeet_network"
}

# Create Redis container
resource "docker_container" "redis" {
  name  = "redis"
  image = docker_image.redis.image_id
  ports {
    internal = 6379
    external = 6379
  }
  networks_advanced {
    name = docker_network.geekmeet_network.name
  }
}

resource "docker_image" "redis" {
  name = "redis:latest"
}

# Create SQL Server container
resource "docker_container" "sqlserver" {
  name  = "sqlserver"
  image = docker_image.sqlserver.image_id
  env = [
    "ACCEPT_EULA=Y",
    "SA_PASSWORD=YourStrong!Passw0rd"
  ]
  ports {
    internal = 1433
    external = 1433
  }
  networks_advanced {
    name = docker_network.geekmeet_network.name
  }
}

resource "docker_image" "sqlserver" {
  name = "mcr.microsoft.com/mssql/server:2022-latest"
}

# Create API container
resource "docker_container" "api" {
  name  = "geekmeet-api"
  image = docker_image.api.image_id
  env = [
    "ASPNETCORE_ENVIRONMENT=Development",
    "ConnectionStrings__DefaultConnection=Server=sqlserver;Database=GeekMeet;User=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True",
    "ConnectionStrings__Redis=redis:6379"
  ]
  ports {
    internal = 80
    external = 8080
  }
  networks_advanced {
    name = docker_network.geekmeet_network.name
  }
  depends_on = [
    docker_container.redis,
    docker_container.sqlserver
  ]
}

resource "docker_image" "api" {
  name = "geekmeet-api:latest"
  build {
    context = ".."
    dockerfile = "Dockerfile"
  }
} 