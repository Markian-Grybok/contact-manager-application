using ContactManager.WebAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreConfiguration(builder.Configuration);

var app = builder.Build();

app.UseCoreConfiguration();

app.Run();
