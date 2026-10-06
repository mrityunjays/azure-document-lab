using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DocumentApi.Tests;

/// <summary>
/// Verifies the API's health response and unmatched-route behavior.
/// </summary>
public class HealthEndpointTests
{
    [Fact]
    public async Task GivenHealthEndpoint_WhenRequested_ReturnsExpectedJson()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);

        Assert.Equal(
            "ok",
            json.RootElement.GetProperty("status").GetString());

        Assert.Equal(
            "DocumentApi",
            json.RootElement.GetProperty("service").GetString());
    }

    [Fact]
    public async Task GivenUnknownRoute_WhenRequested_ReturnsNotFound()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GivenNoToken_WhenAuthCheckRequested_ReturnsUnauthorized()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/auth-check");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}