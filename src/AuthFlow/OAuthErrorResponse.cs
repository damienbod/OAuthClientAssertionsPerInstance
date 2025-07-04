﻿using System.Text.Json.Serialization;

namespace AuthFlow;

/// <summary>
/// https://www.rfc-editor.org/rfc/rfc6749#section-5.2
/// </summary>
public class OAuthErrorResponse
{
    /// <summary>
    /// invalid_request
    /// invalid_client
    /// invalid_grant
    /// unauthorized_client
    /// unsupported_grant_type
    /// invalid_scope
    /// </summary>
    [JsonPropertyName(OAuthConsts.RESPONSE_ERROR)]
    public string? error { get; set; }

    [JsonPropertyName(OAuthConsts.RESPONSE_ERROR_DESCRIPTION)]
    public string? error_description { get; set; }

    [JsonPropertyName(OAuthConsts.RESPONSE_ERROR_URI)]
    public string? error_uri { get; set; }

    // NON spec, additional optional

    [JsonPropertyName(OAuthConsts.RESPONSE_ERROR_CODES)]
    public List<int>? error_codes { get; set; } = new List<int>();
    [JsonPropertyName(OAuthConsts.RESPONSE_ERROR_TIMESTAMP)]
    public DateTime? timestamp { get; set; }
    [JsonPropertyName(OAuthConsts.RESPONSE_ERROR_TRACE_ID)]
    public string? trace_id { get; set; }
    [JsonPropertyName(OAuthConsts.RESPONSE_ERROR_CORRELATION_ID)]
    public string? correlation_id { get; set; }

    /// <summary>
    /// json format "{\"access_token\":{\"polids\":{\"essential\":true,\"values\":[\"9ab03e19-ed42-4168-b6b7-7001fb3e933a\"]}}}"
    /// </summary>
    [JsonPropertyName(OAuthConsts.RESPONSE_ERROR_CLAIMS)]
    public string? claims { get; set; }
}

