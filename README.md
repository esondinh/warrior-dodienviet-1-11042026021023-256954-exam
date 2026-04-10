# Task — .NET Clean Architecture (net8.0)

## Structure
```
src/
  Task.Domain/         # Entities & interfaces
  Task.Application/    # Business logic / services
  Task.Infrastructure/ # Repository implementations
  Task.API/            # Web API entry point
```

## Run
```bash
cd src/Task.API
dotnet run
```
Open http://localhost:5000/swagger
