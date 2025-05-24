# UserDistributed

UserDistributed is a modern, distributed .NET application designed to demonstrate scalable, high-performance user management and analytics. The project leverages:

- **.NET 8** for backend API development
- **Entity Framework Core** for data access with MSSQL
- **Redis** for distributed caching and coordination
- **Kubernetes** and **Docker** for container orchestration and deployment
- **AutoMapper** for clean mapping between domain models and DTOs
- **Parallel programming** for efficient, concurrent data processing

## Goals
- Efficiently manage and process user data at scale
- Demonstrate best practices for distributed systems in .NET
- Showcase modern C# features (records, primary constructors, global usings, etc.)
- Provide robust APIs for user CRUD, search, statistics, and metrics
- Enable easy local development and cloud deployment

## Features
- User CRUD operations
- Parallel and distributed user processing
- Search and analytics endpoints
- Caching and coordination with Redis
- Scalable deployment with Docker and Kubernetes

## Getting Started
1. Clone the repository
2. Set up MSSQL and Redis (see docker-compose or Kubernetes manifests)
3. Run the application locally or deploy to your cluster

---

This repository is a reference for building scalable, cloud-native .NET applications with modern C# and distributed system patterns.
