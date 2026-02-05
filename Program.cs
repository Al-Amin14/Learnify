using learnify.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
string Connectionstring = builder.Configuration.GetConnectionString("Default") ?? throw new ArgumentException("Connection String is null");
builder.Services.AddDbContext<AppDbContenxt>(op => op.UseSqlServer(Connectionstring));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
