using System.Text.Json.Serialization;

namespace AuthFlow;

public class AuthFlowConfiguration
{
    // assertion parameter token validation
    [JsonPropertyName("AccessTokenMetadataAddress")]
    public string AccessTokenMetadataAddress { get; set; } = string.Empty;
    [JsonPropertyName("AccessTokenAuthority")]
    public string AccessTokenAuthority { get; set; } = string.Empty;
    [JsonPropertyName("AccessTokenAudience")]
    public string AccessTokenAudience { get; set; } = string.Empty;

    [JsonPropertyName("Audience")]
    public string Audience { get; set; } = string.Empty;
    [JsonPropertyName("ClientId")]
    public string ClientId { get; set; } = string.Empty;

}
