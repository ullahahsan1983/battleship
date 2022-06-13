using Battleship.Core;
using Battleship.Core.Abstractions;
using Battleship.Domain;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IFleetDesigner, FleetDesigner>();
builder.Services.AddTransient<IPlayerSimulator, PlayerSimulator>();
builder.Services.AddTransient<IGameEngine, GameEngine>();

builder.Services.AddSingleton<IGameBoardProvider, GameBoardProvider>();
builder.Services.AddSingleton<IGameEnvironment, GameEnvironment>();
builder.Services.AddSingleton<IGridPlannerFactory, GridPlannerFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(options =>
{
    // For demonstration only
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});

app.MapControllers();

app.Run();

public partial class Program { }