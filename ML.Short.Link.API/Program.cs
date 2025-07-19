using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ML.Short.Link.API.Data;
using ML.Short.Link.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(_ => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<UrlShortenerService>();
builder.Services.AddScoped<IUrlRepositorio, UrlRepositorio>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "ShortLink API", Version = "v1" });
});

var app = builder.Build();

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
