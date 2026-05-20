using GasWebProject.Models;
using GasWebProject.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// === 1. РЕГИСТРАЦИЯ СЕРВИСОВ (Всё добавляем в builder) ===

builder.Services.AddDbContext<GasDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IGasCalculatorService, GasCalculatorService>();

builder.Services.AddControllers();

// Вот эти две строчки критически важны для Swagger!
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // <--- Из-за отсутствия этой строки падает ошибка    

builder.Services.AddControllers();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
