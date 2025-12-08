using Microsoft.OpenApi.Models;
using RickAndMortyZrpChallenge.Application.Services;
using RickAndMortyZrpChallenge.Domain.Repositories;
using RickAndMortyZrpChallenge.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ZRP Rick and Morty Challenge API",
        Version = "v1",
        Description = "MVP API that wraps the Rick and Morty public API."
    });
});

// HttpClient for Rick and Morty API
builder.Services.AddHttpClient("RickAndMorty", client =>
{
    var baseUrl = builder.Configuration["RickAndMorty:BaseUrl"] ?? "https://rickandmortyapi.com/api/";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IRickAndMortyRepository, RickAndMortyRepository>();
builder.Services.AddScoped<IRickAndMortyService, RickAndMortyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ZRP Rick and Morty Challenge API v1");
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
