namespace AuthFlow;

public class OAuthConsts
{
    public const string ERROR_INVALID_REQUEST = "invalid_request";
    public const string ERROR_INVALID_CLIENT = "invalid_client";
    public const string ERROR_INVALID_GRANT = "invalid_grant";
    public const string ERROR_UNAUTHORIZED_CLIENT = "unauthorized_client";
    public const string ERROR_UNSUPPORTED_GRANT_TYPE = "unsupported_grant_type";
    public const string ERROR_INVALID_SCOPE = "invalid_scope";

    public const string REQUEST_GRANT_TYPE = "grant_type";
    public const string REQUEST_PUBLIC_KEY = "public_key";
    public const string REQUEST_STATE = "state";
    public const string REQUEST_CLIENT_ID = "client_id";
    public const string REQUEST_NONCE = "nonce";

    public const string RESPONSE_TOKEN_TYPE = "token_type";
    public const string RESPONSE_EXPIRES_IN = "expires_in";
    public const string RESPONSE_FP_TOKEN = "fp_token";
 
    // https://www.rfc-editor.org/rfc/rfc6749#section-5.2
    public const string RESPONSE_ERROR = "error";
    public const string RESPONSE_ERROR_DESCRIPTION = "error_description";
    public const string RESPONSE_ERROR_URI = "error_uri";

    // extra claims in error response but not in spec
    public const string RESPONSE_ERROR_CODES = "error_codes";
    public const string RESPONSE_ERROR_TIMESTAMP = "timestamp";
    public const string RESPONSE_ERROR_TRACE_ID = "trace_id";
    public const string RESPONSE_ERROR_CORRELATION_ID = "correlation_id";
    public const string RESPONSE_ERROR_CLAIMS = "claims";

}
