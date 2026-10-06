var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "DocumentApi"
}));

app.Run();

/// <summary>
/// Exposes the API entry point to the integration test host.
/// </summary>
public partial class Program
{
}