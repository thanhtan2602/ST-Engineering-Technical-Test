using Catalog;

var builder = WebApplication.CreateBuilder(args);

// Add service to the container
builder.Services.AddCatalogModule(builder.Configuration);






var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCatalogModule();

app.Run();
