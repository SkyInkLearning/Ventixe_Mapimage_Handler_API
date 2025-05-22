using Application.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var configSection = builder.Configuration.GetSection("ConnectionStrings");
var connectionString = configSection["AzureBlobStorage"];
var container = configSection["Container"];

builder.Services.AddScoped<IMapService>(_ => new MapService(connectionString!, container!));


var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
