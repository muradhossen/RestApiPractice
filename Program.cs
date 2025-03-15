using Movies.Application;
using Movies.Application.Database;

var builder = WebApplication.CreateBuilder(args);
 
var confg = builder.Configuration;

builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication()
    .AddDatabase(confg["Database:ConnectionString"]);

var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var initializer = app.Services.GetRequiredService<DbInitializer>();

await initializer.InitializeAsync();

app.Run();
