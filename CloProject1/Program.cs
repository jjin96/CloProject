using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Reflection;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Bl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddLog4Net();
log4net.GlobalContext.Properties["LogFileName"] = Path.Combine(builder.Environment.ContentRootPath, "Data", "Log");
log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("configPath"));
builder.Services.AddTransient<IEmployeeService, EmployeeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

string basePath = Path.Combine(builder.Environment.ContentRootPath, "Data");
string csvFile = Path.Combine(basePath, "Employee.csv");
string jsonFile = Path.Combine(basePath, "Employee.json");

if (!Directory.Exists(basePath))
{
    Directory.CreateDirectory(basePath);
}

if (!File.Exists(csvFile))
{
    File.Create(csvFile);
}

if (!File.Exists(jsonFile))
{
    File.Create(jsonFile);
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name : "default",
        pattern: "api/{controller=Employee}/{id?}");        
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
