using Carter;
using Catalog;
using Microsoft.OpenApi.Models;
using Shared.Exceptions.Handler;
using Shared.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Modules & building blocks
var catalogAssembly = typeof(CatalogModule).Assembly;
builder.Services.AddCarterWithAssemblies(catalogAssembly);
builder.Services.AddMediatRWithAssemblies(catalogAssembly);
builder.Services.AddAuthorization();

// Serialize/deserialize enums as strings in all Minimal API JSON bindings.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("ETag")));

builder.Services.AddCatalogModule(builder.Configuration);

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FashionShop Catalog API",
        Version = "v1",
        Description = "Product management API — ST Engineering .NET Developer Test"
    });
});

// Cross-cutting: global exception handler + ProblemDetails
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Static files for uploaded product images
app.Environment.WebRootPath ??= Path.Combine(app.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, "uploads", "products"));
app.UseStaticFiles();

// HTTP pipeline
app.UseExceptionHandler(_ => { });
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FashionShop Catalog API v1"));
}

// All routes under /api/v1
app.MapGroup("/api/v1").MapCarter();

app.UseCatalogModule();

app.Run();
