using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger
builder.Services.AddSwaggerGen();

// Database connection string
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string"
        + "'DefaultConnection' not found.");

// Register the DbContext with the dependency injection container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register the Identity DbContext with the dependency injection container
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
   options.UseSqlServer(connectionString));

// Add authorization services to dependency injection container
builder.Services.AddAuthorization();

// Activate Identity APIs
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Explicitly add authentication to the middleware pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<IdentityUser>();

app.MapControllers();

app.Run();
