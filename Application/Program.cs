using Application.Service;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v.1.0",
        Title = "Event MapImageHandler API Documentation",
        Description = "Documentation for the MapImageHandler API."
    });
    o.EnableAnnotations();
    o.ExampleFilters();

    var apiScheme = new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Description = "API KEY",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme",
        Reference = new OpenApiReference
        {
            Id = "ApiKey",
            Type = ReferenceType.SecurityScheme,
        }
    };

    o.AddSecurityDefinition("ApiKey", apiScheme);
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { apiScheme, new List<string>() }
    });
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

var configSection = builder.Configuration.GetSection("ConnectionStrings");
var connectionString = configSection["AzureBlobStorage"];
var container = configSection["Container"];

builder.Services.AddScoped<IMapService>(_ => new MapService(connectionString!, container!));


var app = builder.Build();


app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event ImageHandler API v.1.0");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthorization();
app.MapControllers();

app.Run();
