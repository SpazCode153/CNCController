using API.Machines;
using API.Connectors;
using System.Net;
using Serilog;

Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<MarlinMachineHandler>();
builder.Services.AddSingleton<CommunicationConnector>();

builder.Services.AddControllers();

builder.Services.AddControllers();

builder.Services.AddOptions();

builder.Services.AddCors();

builder.Services.AddSignalR();

builder.Services.AddMvc();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5020);
});

var app = builder.Build();

app.Services.GetService<CommunicationConnector>();

app.UseRouting();

app.UseCors(
    options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
);

app.UseAuthorization();

app.UseWebSockets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    endpoints.MapHub<API.Hubs.CommunicationHub>("/communicationhub");
});

app.MapControllers();

app.Run();
