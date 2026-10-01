using System.Net.Http.Json;

// Create handler that accepts self-signed HTTPS cert (local dev only)
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
};

var http = new HttpClient(handler)
{
    BaseAddress = new Uri("https://localhost:7189/")
};

Console.WriteLine("Calling GET /api/questions...");

try
{
    var response = await http.GetAsync("api/v1/questions");
    Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");

    var json = await response.Content.ReadAsStringAsync();
    Console.WriteLine("Response:");
    Console.WriteLine(json);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

