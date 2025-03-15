using ConsoleClient;
using Duende.IdentityModel;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

Console.Title = "Client assertion Client";

var privatePem = File.ReadAllText(Path.Combine("", "rsa256-private.pem"));
var publicPem = File.ReadAllText(Path.Combine("", "rsa256-public.pem"));

var rsaCertificate = X509Certificate2.CreateFromPem(publicPem, privatePem);
var rsaCertificateKey = new RsaSecurityKey(rsaCertificate.GetRSAPrivateKey());

"\n\nObtaining access token for mobile client".ConsoleYellow();

var response = await RequestTokenAsync(
    new SigningCredentials(rsaCertificateKey, SecurityAlgorithms.RsaSha256));

response.Show();
Console.ReadLine();

"\n\nCalling API".ConsoleYellow();
await CallServiceAsync(response.AccessToken);
Console.ReadLine();

//static async Task<TokenResponse> RequestTokenAsync(string clientId = "mobile-client", string clientSecret = "secret")
//{
//    var client = new HttpClient();

//    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
//    if (disco.IsError) throw new Exception(disco.Error);

//    var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
//    {
//        Address = disco.TokenEndpoint,

//        ClientId = clientId,
//        ClientSecret = clientSecret,
//    });

//    if (response.IsError) throw new Exception(response.Error);
//    return response;
//}

static async Task<TokenResponse> RequestTokenAsync(SigningCredentials credential)
{
    var client = new HttpClient();

    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
    if (disco.IsError) throw new Exception(disco.Error);

    var clientToken = CreateClientToken(credential, "mobile-client", disco.TokenEndpoint);

    var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = disco.TokenEndpoint,
        Scope = "scope-dpop",
        ClientId = "mobile-client",
        ClientCredentialStyle = ClientCredentialStyle.PostBody,

        ClientAssertion =
        {
            Type = OidcConstants.ClientAssertionTypes.JwtBearer,
            Value = clientToken
        }
    });

    if (response.IsError) throw new Exception(response.Error);
    return response;
}

static string CreateClientToken(SigningCredentials credential, string clientId, string tokenEndpoint)
{
    var now = DateTime.UtcNow;

    var token = new JwtSecurityToken(
        clientId,
        tokenEndpoint,
        new List<Claim>()
        {
            new Claim(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
            new Claim(JwtClaimTypes.Subject, clientId),
            new Claim(JwtClaimTypes.IssuedAt, now.ToEpochTime().ToString(), ClaimValueTypes.Integer64)
        },
        now,
        now.AddMinutes(1),
        credential
    );

    var tokenHandler = new JwtSecurityTokenHandler();
    return tokenHandler.WriteToken(token);
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