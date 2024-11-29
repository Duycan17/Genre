using WebApplication1.Data;
using WebApplication1.Endpoints;
using WebApplication1.Entities;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSqlite<GameStoreContext>(connectionString);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

app.MapGamesEndpoints();
app.MigrateDb();
app.Run();