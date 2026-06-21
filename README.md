# TicketSystem.API

A small ticket system Web API built with ASP.NET Core, Entity Framework Core, SQL Server, and ASP.NET Identity.

The purpose of this project was to learn and practice back-end development concepts including REST APIs, authentication, authorization, Entity Framework Core, DTOs, pagination, and working with relational data.

## Technologies

- ASP.NET Core Web API (.NET 10)
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JWT Bearer Authentication
- Swagger / OpenAPI

## Features

### Authentication

- User registration and login with ASP.NET Identity
- JWT Bearer authentication
- Swagger authorization support

### Authorization

- Admin and User roles
- Owner or admin authorization for tickets
- Protection against removing the last administrator

### Tickets Controller

- Create tickets
- View tickets
- Update tickets
- Update ticket status
- Delete tickets (Admin only)

Each ticket stores:

- CreatedAt
- UpdatedAt
- CreatedBy
- UpdatedBy
- Priority
- Status

### Comments Controller

- Create comments
- View comments
- Delete comments

### Users Controller

Admin endpoints for:

- View all users
- Change a user's role

## API Design

Responses use DTOs instead of exposing Entity Framework entities directly.

Collections return pagination information:

- PageNumber
- PageSize
- TotalCount
- TotalPages
- HasNextPage
- HasPreviousPage

## Database

### AppUser (Inherits From ASP.NET Identity User)

One user can create many tickets and many comments.

### Ticket

One ticket belongs to one user and can have many comments.

### Comment

One comment belongs to one ticket and one user.

## Notes

One thing I wanted to understand while building this project was how Entity Framework Core handles related data.

When projecting directly into a DTO, EF Core automatically generates the required SQL JOINs when navigation properties are referenced.

When loading an entity first for business logic (such as validating that the current user owns the ticket before allowing updates), `.Include()` is used to eagerly load the related navigation properties.

Thinking about what SQL code EF Core is generating made a lot of the framework make more sense.

## What I practiced

- REST API design
- Entity Framework Core
- LINQ
- SQL Server
- ASP.NET Identity
- JWT authentication
- Role-based authorization
- Resource ownership validation
- DTO mapping
- Pagination
- Relational database design

This project is intended as a small learning project and portfolio piece focused on back-end development rather than a production-ready ticketing system.
