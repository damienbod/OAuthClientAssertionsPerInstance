using ConsoleClient;
using Duende.IdentityModel;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

Console.Title = "Client assertion Client";

// client assertion
var privatePem = File.ReadAllText(Path.Combine("", "rsa256-private.pem"));
var publicPem = File.ReadAllText(Path.Combine("", "rsa256-public.pem"));
var rsaCertificate = X509Certificate2.CreateFromPem(publicPem, privatePem);
var rsaCertificateKey = new RsaSecurityKey(rsaCertificate.GetRSAPrivateKey());
var signingCredentials = new SigningCredentials(new X509SecurityKey(rsaCertificate), "RS256");

"\n\nObtaining access token for mobile client".ConsoleYellow();

var response = await RequestTokenAsync(signingCredentials);

response.Show();
Console.ReadLine();

"\n\nCalling API".ConsoleYellow();
await CallServiceAsync(response.AccessToken);
Console.ReadLine();


static async Task<TokenResponse> RequestTokenAsync(SigningCredentials signingCredentials)
{
    var client = new HttpClient();

    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
    if (disco.IsError) throw new Exception(disco.Error);

    var clientToken = CreateClientToken(signingCredentials, "mobile-client", disco.Issuer);
    var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = disco.TokenEndpoint,

        ClientAssertion =
        {
            Type = OidcConstants.ClientAssertionTypes.JwtBearer,
            Value = clientToken
        },

        Scope = "mobile", 
    });

    if (response.IsError) throw new Exception(response.Error);
    return response;
}

static string CreateClientToken(SigningCredentials credential, string clientId, string audience)
{
    var now = DateTime.UtcNow;

    var token = new JwtSecurityToken(
        clientId,
        audience,
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
    var clientToken = tokenHandler.WriteToken(token);
    "\n\nClient Authentication Token:".ConsoleGreen();
    Console.WriteLine(token);
    return clientToken;
}

static async Task CallServiceAsync(string token)
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:5003/")
    };

    client.SetBearerToken(token);
    var response = await client.GetStringAsync("identity");

    "\n\nService claims:".ConsoleGreen();
    Console.WriteLine(response.PrettyPrintJson());
}