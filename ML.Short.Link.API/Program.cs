var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<ML.Short.Link.API.Services.UrlShortenerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "ShortLink API", Version = "v1" });
});


var app = builder.Build();


//app.MapPost("api/shorten", (string url, IUrlShortener shortener) =>
//{
//    var shortUrl = shortener.ShortenUrl(url);
//    return Results.Ok(shortUrl);
//});

//app.MapGet("/{shortCode}", (string shortCode, IUrlShortener shortener) =>
//{
//    var originalUrl = shortener.Redirect(shortCode);
//    return Results.Redirect(originalUrl);
//});


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
