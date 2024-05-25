using AuthServer.Core.Configuration;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); //appsettingjsona gidip

var tokenOptionsSection = builder.Configuration.GetSection("TokenOptions"); //appsettingjson içeriðindeki json nesnesini getir
builder.Services.Configure<CustomTokenOptions>(tokenOptionsSection); //sonrada configure et


var clients = builder.Configuration.GetSection("Clients"); //appsettingjson içeriðindeki json nesnesini getir
builder.Services.Configure<List<Client>>(clients); //sonrada configure et

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
