using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pustakalaya.Data;
using Pustakalaya.Services.Interface;
using Pustakalaya.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Enhanced Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Pustakalaya API",
        Version = "v1",
        Description = "API for Book Library System"
    });

    // JWT Authentication configuration for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer [space] your token' in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            new string[] {}
        }
    });
});

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add Database Context with error handling
try
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    Console.WriteLine("Database connection configured successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Error configuring database connection: {ex.Message}");
}

// Register Authentication Service
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Authentication Service
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Book and Announcement Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? "FallbackKeyIfSecretIsNotFound123456789012345678901234"))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pustakalaya API v1");
        c.RoutePrefix = "swagger";
    });

    // Simple error handling in development
    app.UseDeveloperExceptionPage();
}

// Use CORS before other middleware
app.UseCors();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add a simple test endpoint to verify API is working
app.MapGet("/api/test", () => "Pustakalaya API is working!");

// Add Authentication middleware before Authorization
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapFallbackToFile("login.html");

Console.WriteLine("Application starting...");

app.Run();