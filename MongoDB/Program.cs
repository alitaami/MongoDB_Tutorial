using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using MongoDB.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "app", Version = "v1" });
});
 
builder.Services.AddSingleton<MongoDBContext>();
 
var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();
 
app.MapControllers();
 
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name");
    c.InjectStylesheet("/swagger/swagger-ui.css"); // Check this line
});

app.Run();
