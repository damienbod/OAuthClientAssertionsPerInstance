using System.Text.Json.Serialization;

namespace AuthFlow;

public class DeviceRegistrationResponse
{
    //"fp_token": "2YotnFZFEjr1zCsicMWpAA",
    //"token_type": "fp+jwt",
    //"state": "<state>"
    //"expires_in": 420

    [JsonPropertyName(OAuthConsts.RESPONSE_FP_TOKEN)]
    public string FpToken { get; set; } = string.Empty;

    [JsonPropertyName(OAuthConsts.RESPONSE_TOKEN_TYPE)]
    public string TokenType { get; set; } = OAuthConsts.TOKEN_TYPE;

    [JsonPropertyName(OAuthConsts.REQUEST_STATE)]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName(OAuthConsts.RESPONSE_EXPIRES_IN)]
    public string ExpiresIn { get; set; } = "420";
}