using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoeStore.Components;
using ShoeStore.Data;
using ShoeStore.Auth;
using ShoeStore.Maping;
using System.Text;
using Blazored.LocalStorage;
using ShoeStore.Servicess;
using ShoeStore.Servicess.Impl;
using log4net.Config;
using log4net;
using System.Reflection;
using ShoeStore.Components.ApiService;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


// Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true));
        // options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; // if circular references needed
    });

// Razor Components

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Configure Log4Net
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("DefaultConnection is not set in configuration.");
}

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
});

// JWT Authentication
var jwtConfig = builder.Configuration.GetSection("JwtSettings");
var secret = jwtConfig["Secret"];
if (string.IsNullOrEmpty(secret))
{
    throw new InvalidOperationException("JWT:Secret is not set in configuration.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ClockSkew = TimeSpan.Zero,
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["accessToken"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/_blazor"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped<IAuthHeaderService, AuthHeaderService>();

// Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage(config =>
{
    config.JsonSerializerOptions.WriteIndented = true;
});


// Services registration
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<ApiServices>();
builder.Services.AddScoped<IDataSerializer, DataSerializer>();
builder.Services.AddSingleton<AppState>();


// HttpClient registration using IHttpClientFactory (recommended)
builder.Services.AddHttpClient<ApiServices>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7153/");
});

// Authorization policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN"))
    .AddPolicy("AdminOrManager", policy => policy.RequireRole("ADMIN", "MANAGER"))
    .AddPolicy("AdminOrManagerOrUser", policy => policy.RequireRole("ADMIN", "MANAGER", "USER"))
    .AddPolicy("User", policy => policy.RequireRole("USER"));

// Controllers
builder.Services.AddControllers();



// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



// ------------------ Build App ------------------
var app = builder.Build();

// Apply EF migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}



// ------------------ Middleware Pipeline ------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery(); // ✅ Must be placed here (AFTER Authentication & Authorization, BETWEEN UseRouting and MapEndpoints)

//Map endpoints
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ShoeStore.Client._Imports).Assembly);

app.Run();
