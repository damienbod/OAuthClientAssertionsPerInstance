using System.Text.Json.Serialization;

namespace AuthFlow;

public class AuthFlowConfiguration
{
    // assertion parameter token validation
    [JsonPropertyName("TokenMetadataAddress")]
    public string TokenMetadataAddress { get; set; } = string.Empty;
    [JsonPropertyName("TokenAuthority")]
    public string TokenAuthority { get; set; } = string.Empty;
    [JsonPropertyName("ClientId")]
    public string ClientId { get; set; } = string.Empty;
}
