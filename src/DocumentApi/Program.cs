using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

if (!Guid.TryParse(builder.Configuration["AzureAd:TenantId"], out _) ||
    !Guid.TryParse(builder.Configuration["AzureAd:ClientId"], out _))
{
    throw new InvalidOperationException(
        "Configure AzureAd:TenantId and AzureAd:ClientId with actual GUID values.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(
        builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DocumentApiAccess", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireScope("access_as_user");
    });
});

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => TypedResults.Ok(new
{
    status = "ok",
    service = "DocumentApi"
}))
    .AllowAnonymous()
    .WithSummary("Checks application availability.");

app.MapGet("/auth-check", () =>
    TypedResults.Ok(new AuthCheckResponse("Access granted")))
    .RequireAuthorization("DocumentApiAccess")
    .WithSummary("Checks authenticated access with the required API scope.");

app.Run();

/// <summary>
/// Represents a successful authorization check.
/// </summary>
/// <param name="Message">The authorization result message.</param>
public sealed record AuthCheckResponse(string Message);

/// <summary>
/// Exposes the API entry point to the integration test host.
/// </summary>
public partial class Program
{
}