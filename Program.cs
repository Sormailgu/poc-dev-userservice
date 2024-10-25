using UserService.Services;
using FinancialService.Client;
using ConfigureService.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Http.HttpClientLibrary;

var builder = WebApplication.CreateBuilder(args);

// Register BackgroundSyncService with a specific interval
builder.Services.AddSingleton<BackgroundSyncService>(provider => 
{
    var service = new BackgroundSyncService(provider.GetRequiredService<FinancialServiceClient>(), TimeSpan.FromSeconds(5));
    return service;
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Kiota handlers to the dependency injection container
builder.Services.AddKiotaHandlers();

// Register the factory for the FinancialService client
builder.Services.AddHttpClient<FinancialServiceClientFactory>((sp, client) => {
    // Set the base address for the FinancialService API
    client.BaseAddress = new Uri("http://localhost:5180/api");
}).AttachKiotaHandlers(); // Attach the Kiota handlers to the HTTP client

// Register the FinancialService client
builder.Services.AddTransient(sp => sp.GetRequiredService<FinancialServiceClientFactory>().GetClient());

// Register the factory for the ConfigureService client
builder.Services.AddHttpClient<ConfigureServiceClientFactory>((sp, client) => {
    // Set the base address for the ConfigureService API
    client.BaseAddress = new Uri("http://localhost:5102");
}).AttachKiotaHandlers(); // Attach the Kiota handlers to the HTTP client

// Register the ConfigureService client
builder.Services.AddTransient(sp => sp.GetRequiredService<ConfigureServiceClientFactory>().GetClient());



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();