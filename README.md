# ToDoApp
# To-Do List API (ASP.NET Core)

## Description:
This project is a custom API for managing daily tasks (To-Do Lists), built using **ASP.NET Core** with **Onion Architecture**. The system supports API authentication using **Swagger** and provides role-based user permissions using **JWT Authentication**.

## Requirements:
- .NET Core 5.0 or higher
- SQL Server
- Docker (if you want to run the environment using Docker Compose)

## Key Features:
- Task management (add, edit, delete, browse). And check if title exist in database before on Create And Update.
- Users and roles have been added in addition to three primary tasks within SeedData.
- Use Auto Mapper and Fluent Validation (CreateToDoItemDtoValidator, ToDoItemDtoValidator, UpdateToDoItemDtoValidator ).
- Mark tasks as "complete" or "incomplete".
- Filtering, searching, and sorting support.
- Pagination support.
- Role support: **Owner** and **Guest**. with polices (CanViewTasks, CanEditTasks, CanDeleteTasks, CanCreateTasks).
- **Swagger UI** for API testing.
- Advanced authentication using **JWT**.

## How to run:

1. **Environment setup:**
- Upload the project using the following command:
```bash
git clone git clone https://github.com/BahaaEbraheem/ToDoApp.git
```
- Open the project in **Visual Studio** or use **VS Code**.

2. **Database Setup:**
- Use **EF Core Migrations** to create the database:
```bash
dotnet ef database update
```

3. **Run the Project:**
- To run the project in the local environment:
```bash
dotnet run
```
- The project can also be run via **Docker Compose**:
```bash
docker-compose up
```

## API Testing:
- You can use **Postman** or **Swagger UI** to test the API:
- **Swagger UI** will be available when the project is run on `https://localhost:7112/swagger/index.html`.
- **Postman Collection** is available in the following path: `https://github.com/BahaaEbraheem/ToDoApp/blob/master/postman/Elkood_ToDoListAPI.postman_collection.json`.
- **Postman Collection** is available in the following path: `https://github.com/BahaaEbraheem/ToDoApp/blob/master/postman/ Elkood.postman_test_run.json`.
- All tested AIPs correctly and I put the test link here for you to run after adding the token but pay attention to the token expiration time

## Technologies and Tools Used:
- **ASP.NET Core Web API**
- **Onion Architecture**
- **JWT Authentication**
- **EF Core**
- **Swagger**
- **Docker**
- **AutoMapper**
- **FluentValidation**
- **Unit Tests / Integration Tests**

## How to Add Seed Data:
When the system is first started, a seed data will be created that includes a user of type **Owner** and some test tasks.

## Instructions for Running Docker Compose:
To run the environment using **Docker Compose**, ensure that Docker is installed on your machine. Then run the following command:

```bash
docker-compose up
