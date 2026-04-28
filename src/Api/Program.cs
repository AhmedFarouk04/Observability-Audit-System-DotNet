using Api.Extensions;
using Bootstrapper;
using Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogConfiguration();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseInfrastructure();
app.UseApiApplication();

app.Run();

public partial class Program;
