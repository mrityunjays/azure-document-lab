using System.Net.Http.Headers;
using Microsoft.Identity.Client;

if (args.Length != 3 ||
    !Guid.TryParse(args[0], out var tenantId) ||
    !Guid.TryParse(args[1], out var clientId) ||
    !Guid.TryParse(args[2], out var apiId))
{
    throw new ArgumentException(
        "Provide three GUIDs: tenant ID, document-lab-client ID, document-lab-api ID.");
}

var identityClient = PublicClientApplicationBuilder
    .Create(clientId.ToString())
    .WithAuthority(AzureCloudInstance.AzurePublic, tenantId.ToString())
    .WithRedirectUri("http://localhost")
    .Build();

string[] scopes = [$"api://{apiId}/access_as_user"];

Console.WriteLine("Opening your browser for sign-in...");

var authentication = await identityClient
    .AcquireTokenInteractive(scopes)
    .WithUseEmbeddedWebView(false)
    .ExecuteAsync();

Console.WriteLine("Access token acquired. Calling the Azure API...");

using var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://docapi-mrityunjays-hth6eca4avfdezfe.centralindia-01.azurewebsites.net"),
    Timeout = TimeSpan.FromSeconds(30)
};

using var request = new HttpRequestMessage(HttpMethod.Get, "/auth-check");

request.Headers.Authorization = new AuthenticationHeaderValue(
    "Bearer",
    authentication.AccessToken);

using var response = await httpClient.SendAsync(request);
var body = await response.Content.ReadAsStringAsync();

Console.WriteLine($"HTTP {(int)response.StatusCode} {response.StatusCode}");
Console.WriteLine(body);

response.EnsureSuccessStatusCode();