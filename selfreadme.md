C# Code Reviewing.

--
Steps taken to improve the bad code:

- create proper models
- implement repository pattern
- add service layer
- dependency injection
- auth, reduce to just 'add auth layer'
- secure API key -> connection string, startup.cs IConfiguration
- error handler try/catch

--
Kept Anti-patterns:
SQL injection vulnerabilities in all queries
Plain text password storage
No input validation
Hardcoded API key
Hardcoded fallback connection string
Using dynamic types instead of proper DTOs
No parameterized queries
No async/await despite being a web API
No proper error handling
No proper response types
No proper HTTP status codes
No proper logging
No proper security measures

--
Did we:

- cover parallel programming / concurrency
- unit testing


-- 
Testing
Observability
Perf?


--
Concurrency and parallel programming 

PLINQ (Parallel LINQ) for parallel processing
Task.WhenAll for concurrent operations
Parallel.ForEach for parallel iteration
Concurrent collections (ConcurrentBag, ConcurrentDictionary)
SemaphoreSlim for limiting concurrent operations

Considerations for C#:

Use thread pool configuration
Implement circuit breakers
Monitor thread usage
Consider memory management

--
What about scaling with k8s?



--
App

Simple app, one service.
Azure functions & azure service bus with terraform 

Testing free tiers:

# Azure Free Tier Testing Setup
- Azure Functions (Free tier)
- Azure Service Bus (Free tier)
- Azure Storage (Free tier)
- Azure Cosmos DB (Free tier)
- Azure DevOps (Free tier)

Terraform

||

Dotnet Docker/Terraform Kubernetes with shared cache and parallel programming

Testing free tiers:

# 1. Local Development
- Docker Desktop (includes Kubernetes)

# 2. Cloud Options
- Azure Kubernetes Service (AKS) - Free tier includes:
  - 1 cluster
  - 2 nodes
  - 50,000 vCPU-hours/month

