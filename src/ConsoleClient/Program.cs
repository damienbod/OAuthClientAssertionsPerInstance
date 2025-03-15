using ConsoleClient;
using IdentityModel.Client;

Console.Title = "Client assertion Client";

"\n\nObtaining access token for mobile client".ConsoleYellow();
var dynamicClientToken = await RequestTokenAsync("mobile-client", "secret");
dynamicClientToken.Show();
Console.ReadLine();

"\n\nCalling API".ConsoleYellow();
await CallServiceAsync(dynamicClientToken.AccessToken);
Console.ReadLine();

static async Task<TokenResponse> RequestTokenAsync(string clientId = "mobile-client", string clientSecret = "secret")
{
    var client = new HttpClient();

    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
    if (disco.IsError) throw new Exception(disco.Error);

    var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = disco.TokenEndpoint,

        ClientId = clientId,
        ClientSecret = clientSecret,
    });

    if (response.IsError) throw new Exception(response.Error);
    return response;
}

static async Task CallServiceAsync(string token)
{
    var baseAddress = Constants.SimpleApi;

    var client = new HttpClient
    {
        BaseAddress = new Uri(baseAddress)
    };

    client.SetBearerToken(token);
    var response = await client.GetStringAsync("identity");

    "\n\nService claims:".ConsoleGreen();
    Console.WriteLine(response.PrettyPrintJson());
}