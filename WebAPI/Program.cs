using System.Security.Cryptography;
using Application.Infrastructure.AppData;
using Application.Logic;
using Application.Logic.Account;
using Domain.Database;
using Infrastructure.UserRepository;
using Microsoft.OpenApi.Models;
using Utility;
using Utility.Http;
using WebAPI.Configuration;
using WebAPI.Configuration.Db;

var builder = WebApplication.CreateBuilder(args);

string corsName = "CorsName";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsName, policyBuilder => policyBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddSingleton<ApplicationDbContext>();

builder.Services.AddControllersWithViews().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    opt.JsonSerializerOptions.WriteIndented = true;
});

// Auto Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Services
builder.Services.AddScoped<IAccountService, AccountService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Utilities
builder.Services.AddScoped<IExpressionUtility, ExpressionUtility>();
builder.Services.AddScoped<IHttpUtility, HttpUtility>();

// Data
builder.Services.AddSingleton<IAppData, AppData>();
builder.Services.AddScoped<IDbSeedConfig, DbSeedConfig>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", info: new OpenApiInfo { Title = "Dapper Glorious API", Version = "v1" });
    option.OperationFilter<HeaderFilter>();
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

var app = builder.Build();

// Initialize
app.SeedDb();

// Swagger
var swaggerConfig = new SwaggerConfig();
builder.Configuration.GetSection(nameof(SwaggerConfig)).Bind(swaggerConfig);
app.UseSwagger(option => { option.RouteTemplate = swaggerConfig.JsonRoute; });
app.UseSwaggerUI(option => { option.SwaggerEndpoint(swaggerConfig.UIEndpoint, swaggerConfig.Description); });

app.UseTiming();

app.UseCors(corsName);

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseExceptionHandler("/Error");

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();