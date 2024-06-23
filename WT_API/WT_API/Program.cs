using Microsoft.EntityFrameworkCore;
using System;
using WT_API.Data;
using WT_API.Models;
using WT_API.Models.WT_API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnString") ?? throw new InvalidOperationException("Connection string 'ApiContext' not found.")));


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<ScrapingServiceSelenium>();
builder.Services.AddHttpClient<ScrapingServiceHtmlAgilityPack>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

///html/body/div[2]/div/div/div/article/div/p[1]/strong/a[2]
///html/body/div[2]/div/div/div/article/div/p[1]/a
///html/body/div[2]/div/div/div/article/div/p[1]/strong/a[1]
