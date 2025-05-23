﻿using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ToDoApp.API;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Mappings;
using ToDoApp.Application.Services;
using ToDoApp.Application.Validators;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Repositories;
using ToDoApp.Infrastructure.Data;
using ToDoApp.Infrastructure.Repositories;
using ToDoApp.Infrastructure.SeedData;
using static ToDoApp.Infrastructure.SeedData.ToDoSeedData;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile("appsettings.Docker.json", optional: true) // ← أضف هذا السطر
    .AddEnvironmentVariables();

var JwtSecretkey = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]);
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(JwtSecretkey),
    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
    ValidAudience = builder.Configuration["JwtSettings:Audience"],
    ValidateIssuer = true,
    ValidateAudience = true,
    RequireExpirationTime = true,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero // Strict expiration check
};
builder.Services.AddSingleton(tokenValidationParameters);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
       .AddJwtBearer(x =>
       {
           x.RequireHttpsMetadata = false;
           x.SaveToken = true;
           x.TokenValidationParameters = tokenValidationParameters;

       });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Owner", policy => policy.RequireRole("Owner"));
    options.AddPolicy("Guest", policy => policy.RequireRole("Guest"));
});

// إضافة خدمات التفويض مع سياسات الأدوار
builder.Services.AddAuthorization(options =>
{
    // سياسة Owner: يمكنه دعوة مستخدمين آخرين إلى المنصة
    options.AddPolicy("CanInviteUsers", policy => policy.RequireRole("Owner"));

    // سياسة Guest: صلاحيات محدودة مثل عرض المهام فقط
    options.AddPolicy("CanViewTasks", policy => policy.RequireRole("Guest", "Owner"));
    options.AddPolicy("CanEditTasks", policy => policy.RequireRole("Owner"));
    options.AddPolicy("CanDeleteTasks", policy => policy.RequireRole("Owner"));
    options.AddPolicy("CanCreateTasks", policy => policy.RequireRole( "Owner"));
});





builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    );
});

// Add services to the container.
builder.Services.AddControllers()
       .AddFluentValidation(fv =>
       {
           fv.RegisterValidatorsFromAssemblyContaining<ToDoItemDtoValidator>();
           fv.RegisterValidatorsFromAssemblyContaining<CreateToDoItemDtoValidator>();
           fv.RegisterValidatorsFromAssemblyContaining<UpdateToDoItemDtoValidator>();
       }); 
// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Add services for Application Layer (Dependency Injection)
builder.Services.AddScoped<IToDoItemService, ToDoItemService>();
builder.Services.AddScoped<IToDoItemRepository, ToDoItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});







var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API v1");
    });
}
app.UseMiddleware<ErrorHandlingMiddleware>(); // إضافة Middleware الخاص بالأخطاء

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");
// Enable Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();
// Seed the database with initial users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // 👈 Ensure database is created & schema applied

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    int retries = 5;
    for (int i = 0; i < retries; i++)
    {
        try
        {
            await ToDoSeedData.SeedData.Initialize(services, userManager);
            Console.WriteLine("✅ Seeding succeeded.");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Seeding attempt {i + 1} failed: {ex.Message}");
            if (i == retries - 1)
                throw;

            await Task.Delay(5000);
        }
    }
}





app.MapControllers();

app.Run();



record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
