using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ML.Short.Link.API.Data.Interfaz;
using ML.Short.Link.API.Data.Service;
using ML.Short.Link.API.JWT;
using ML.Short.Link.API.Services;
using ML.Short.Link.API.Utils;
using System.Text;
using MaxMind.GeoIP2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(_ => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUrlRepositorio, UrlRepositorio>();
builder.Services.AddScoped<IUserRepositorio, UserRepositorio>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<jwtServices>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UrlShortenerService>();

var geoIpDbPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "GeoLite2-City.mmdb");
if (File.Exists(geoIpDbPath))
{
    builder.Services.AddSingleton<DatabaseReader>(_ => new DatabaseReader(geoIpDbPath));
}
else
{
    Console.WriteLine("⚠️ Advertencia: No se encontró la base de datos GeoLite2. La geolocalización estará desactivada.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "ShortLink API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
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



//JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

 builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false, // Para desarrollo
            ValidateAudience = false // Para desarrollo
        };
    });

builder.Services.AddAuthorization();

    var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
