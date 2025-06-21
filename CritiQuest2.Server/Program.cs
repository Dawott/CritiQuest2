using CritiQuest2.Server.Data;
using CritiQuest2.Server.Model.Entities;
using CritiQuest2.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Convert enums to strings in JSON
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // Optional: Use camelCase for property names
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }); ;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Console.WriteLine($"[JWT] Token received: {(string.IsNullOrEmpty(token) ? "No token" : $"Token present ({token.Length} chars)")}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            Console.WriteLine($"[JWT] Token validated successfully - User ID: {userId}, Email: {email}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[JWT] Authentication failed: {context.Exception.GetType().Name} - {context.Exception.Message}");
            if (context.Exception.InnerException != null)
            {
                Console.WriteLine($"[JWT] Inner exception: {context.Exception.InnerException.Message}");
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"[JWT] Authorization challenge: {context.Error} - {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", builder =>
    {
        
            builder.WithOrigins("https://localhost:59279",
            "https://localhost:5173",
                "http://localhost:5173",
                "https://localhost:3000",
                "http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
        
    });

});

builder.Services.AddScoped<IGachaService, GachaService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IDatabaseSeedingService, DatabaseSeedingService>();
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowVueApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Upewniam sie czy baza istnieje...");
        await context.Database.EnsureCreatedAsync();

        
        if (app.Environment.IsProduction())
        {
            logger.LogInformation("Ruszam z migracjami...");
            await context.Database.MigrateAsync();
        }

        // Seed
        logger.LogInformation("Zaczynam seed bazy...");
        await app.Services.SeedDatabaseAsync();
        logger.LogInformation("Seedin zakonczony!");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetService<ILogger<Program>>();
    logger?.LogError(ex, "Blad seedowania bazy");

    // In development, we might want to see the error
    if (app.Environment.IsDevelopment())
    {
        throw;
    }
}

app.Run();
