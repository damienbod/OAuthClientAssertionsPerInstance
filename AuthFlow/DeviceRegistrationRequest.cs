using System.Text.Json.Serialization;

namespace AuthFlow;

public class DeviceRegistrationRequest
{
    //client_id=cid_235saw4r4
    //&grant_type=fp_register
    //&public_key=<public_key>
    //&state=<state>
    //&nonce=<nonce>

    [JsonPropertyName(OAuthConsts.REQUEST_CLIENT_ID)]
    public string client_id { get; set; } = string.Empty;

    [JsonPropertyName(OAuthConsts.REQUEST_GRANT_TYPE)]
    public string grant_type { get; set; } = string.Empty;

    [JsonPropertyName(OAuthConsts.REQUEST_PUBLIC_KEY)]
    public string public_key { get; set; } = string.Empty;

    [JsonPropertyName(OAuthConsts.REQUEST_ALG)]
    public string alg { get; set; } = string.Empty;

    [JsonPropertyName(OAuthConsts.REQUEST_STATE)]
    public string state { get; set; } = string.Empty;

    [JsonPropertyName(OAuthConsts.REQUEST_NONCE)]
    public string nonce { get; set; } = string.Empty;
}
