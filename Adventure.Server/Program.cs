using Adventure.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddSignalR()
    .AddMessagePackProtocol();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.MapHub<GameHub>("game");

app.Run();
