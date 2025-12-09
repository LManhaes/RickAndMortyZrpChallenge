using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using RickAndMortyZrpChallenge.Application.Services;
using RickAndMortyZrpChallenge.Domain.Repositories;
using RickAndMortyZrpChallenge.Infrastructure.Repositories;
using RickAndMortyZrpChallenge.Application.Validation; // onde está o GetEpisodesRequestValidator

var builder = WebApplication.CreateBuilder(args);

// ==== CORS ====
const string DefaultCorsPolicy = "DefaultCorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(DefaultCorsPolicy, policy =>
    {
        // Lê as origens permitidas do appsettings.json: Cors:AllowedOrigins
        var origins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>();

        if (origins is { Length: > 0 })
        {
            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // Fallback: libera tudo (útil só pra DEV)
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// ===== MVC + FluentValidation =====
builder.Services
    .AddControllersWithViews()
    .AddFluentValidation(fv =>
    {
        // opcional: desabilita validação por DataAnnotations se quiser usar só FluentValidation
        fv.DisableDataAnnotationsValidation = true;
    });

// registra todos os validators a partir da assembly onde está o GetEpisodesRequestValidator
builder.Services.AddValidatorsFromAssemblyContaining<GetEpisodesRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetCharacterRequestValidator>();

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

// ==== CORS antes de Authorization ====
app.UseCors(DefaultCorsPolicy);

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
