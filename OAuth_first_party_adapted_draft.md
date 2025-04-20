# Protocol Overview

There are two primary ways this specification extends various parts of an OAuth system.

## Initial Authorization Request

~~~ ascii-art
                                                +-------------------+
                                                |   Authorization   |
                          (A) Device            |      Server       |
                              Registration      |                   |
             +----------+     Request           |+-----------------+|
(A)Client+---|  First-  |---------------------->||   Registration  ||
   Starts|   |  Party   |                       ||     Endpoint    ||
   App   +-->|  Client  |<----------------------||                 ||
             |          | (B) Auth Session      |+-----------------+|
             |          |     Response          |                   |
             |          |                       |                   |
             |          | (C) Client assertion  |                   |
             |          |     Token Request     |+-----------------+|
             |          |---------------------->||      Token      ||
             |          |                       ||     Endpoint    ||
             |          |<----------------------||                 ||
             |          | (D) Access Token      |+-----------------+|
             |          |     Response          |                   |
             |          |                       |                   |
             |          |                       |                   |
             |          |  (E)Authorization     |                   |
             |          |    Challenge Request  |+-----------------+|
             |          |---------------------->||  Authorization  ||
             |          |                       ||   Challenge     ||
             |          |<----------------------||    Endpoint     ||
             |          | (F)Authorization      ||                 ||
             |          |    Error Response     ||                 ||
             |          |         :             ||                 ||
             |          |         :             ||                 ||
             |          | (G)Authorization      ||                 ||
             |          |    Challenge Request  ||                 ||
             |          |---------------------->||                 ||
             |          |                       ||                 ||
             |          |<----------------------||                 ||
             |          | (H) Authorization     |+-----------------+|
             |          |     Response          |                   |
             |          |                       |                   |
             |          |                       |                   |
             +----------+                       +-------------------+
~~~
Figure: First-Party Client Authorization Device Request

- (A) The first-party client starts application for the first time and creates an asymmetric private, public key pair. The client initiates the authorization request by making a POST request to the Device Registration Endpoint using the public key.
- (B) The Authorization Server determines whether the information provided to the Device Registration Endpoint is sufficient. The server creates an 'auth_session' for the public key and returns the 'auth_session' in the response.
- (C) The Device requests an access token with a client assertion and OAuth client credentials created using the private key. The 'auth_session' is added to the client assertion using the 'device_auth_session' claim. The public key attached to the auth_session is used to validate the client assertion. Optional, DPoP is used to request the token. DPoP does not use the same private, public key pair.
- (D) The Authorization Server returns an access token from the Token Endpoint. The 'auth_session' is returned in the access token. 
- (E) The Authorization Challenge Endpoint is used to attach user authentication properties to the device and the auth_session. The Authorization Server authorises the access token using standard OAuth requirements, including DPoP. The auth_session claim is used to authorize specifics for the user.
- (F) ..
- (G) repeat for n-user properties
- (H) ..

# Protocol Endpoints

## Device Registration Endpoint

The Device Endpoint is used to register devices. A client creates a new private, public key and stores to key to in a safe location for the lifetime of the application.

The client makes a request to the device registration endpoint by adding the following parameters, as well as parameters from any extensions, using the application/x-www-form-urlencoded format with a character encoding of UTF-8 in the HTTP request body:

"client_id": : REQUIRED 

"grant_type": : REQUIRED

"public_key": : REQUIRED. RS256 

"alg": : REQUIRED "ES256" "RS256"

"state": : OPTIONAL. 

"nonce": : OPTIONAL. 

### Device Registration Request

~~~
POST /token HTTP/1.1
Host: as.example.com
...
client_id=<client_id>
&grant_type=urn:ietf:params:oauth:grant-type:fp_register
&public_key=<public_key>
&alg=RS256
&state=<state>
&nonce=<nonce>
 ~~~

## Device Registration Response

"fp_token": : REQUIRED. The signed JWT token. MUST be fully validated included signature.

"token_type": : REQUIRED. "fp+jwt"

"state": : OPTIONAL. MUST be validated if requested. The server MUST included this from the original request.

"expires_in": : REQUIRED. 

An example successful device registration response is below:

~~~
HTTP/1.1 200 OK
Content-Type: application/json
Cache-Control: no-store

{
    "fp_token": "2YotnFZFEjr1zCsicMWpAA",
    "token_type": "fp+jwt",
    "state": "<state>"
    "expires_in": 420
}
~~~

### Device Registration Response fp_token

"auth_session": : REQUIRED. Used for all token requests for this device. This is a unique value. One public_key has one auth_session. The server generates this in a random way.

"aud": : REQUIRED. 

"nonce": : OPTIONAL. MUST be validated if requested. The server MUST included this from the original request.

"iss": : REQUIRED. 

~~~
{
    "alg": "RS256",
    "kid": "9E08135FAEFB9D9E7F7520792656BA0A",
    "typ": "fp+jwt"
}.{
    "iss": "<issuer>",
    "nbf": 1744120238,
    "iat": 1744120238,
    "aud": "<client_id>"
    "exp": 1744123838,
    "auth_session": "<auth_session>",
    "nonce": "<nonce>"
}
~~~

## Token Endpoint

The client assertion is extended to use a new "device_auth_session" parameter and is used to authenticate the token request.

### OAuth Client Credentials token request using client assertion Request

The Authorization Server uses the 'device_auth_session' to find the correct public key to authenticate the client assertion. The value MUST be added to the resulting access token on a successful validation. 

#### Client assertion

"device_auth_session": : REQUIRED. Used to find the device public key and authenticate the device.

~~~
{
  "aud": "https://localhost:5101/connect/token",
  "iss": "onboarding-user-client",
  "exp": 1744142406,
  "jti": "668723c5-7324-4879-a780-83c1edf2232d",
  "sub": "onboarding-user-client",
  "iat": 1744142346,
  "device_auth_session": "<auth_session>",
  "nbf": 1744142346
}
~~~

#### Token request

The auth_session can be requested using the scope.

### Token Endpoint Successful Response

This specification extends the OAuth 2.0 [RFC6749] token response
defined in Section 5.1 with the additional parameter auth_session

The auth_session MUST be included in the access token. The auth_session MUST match the device_auth_session value used in the client assertion.

DPoP SHOULD be used.

An example successful token response is below:

~~~
HTTP/1.1 200 OK
Content-Type: application/json
Cache-Control: no-store

{
    "access_token": "2YotnFZFEjr1zCsicMWpAA",
    "token_type": "Bearer",
    "expires_in": 3600
}
~~~

### Example of Access Token using DPoP

The access token can be used for all further requests.

The auth_session is REQUIRED and used for device authorization.
~~~
{
    "alg": "RS256",
    "kid": "9E08135FAEFB9D9E7F7520792656BA0A",
    "typ": "at+jwt"
}.{
    "iss": "https://localhost:5101",
    "nbf": 1744120238,
    "iat": 1744120238,
    "exp": 1744123838,
    "aud": "https://localhost:5101/resources",
    "cnf": {
        "jkt": "1-xQJRDcRlAGIvAAd1ayQSenXcW5_Ecez_G13qdcM6c"
    },
    "scope": [
        "auth_session:<auth_session>",
        "OnboardingUserScope"
    ],
    "client_id": "onboarding-user-client",
    "jti": "7651BD4201E947DA4220A01D7207F44E"
}
~~~

### Authorization Challenge Request

The Endpoint uses standard OAuth API best practices. No security changes are made to this endpoint from the existing OAuth standards. 

Sender-Constrained Tokens SHOULD be used. The request body can be anything depending on the application authorization requirements.

A phishing resistant user authentication is recommended on the device for the user. 

Possible user authentication methods on the device:

- email
- SMS
- Passkeys
- OTP

~~~
POST /AuthorizationChallengeRequest HTTP/1.1
Host: as.example.com
Authorization: DPoP "access_token"
...
&email=<email_address>
~~~

# Using the access token on a resource server

The auth_session is included in the access token. This claim is used to implement any further Application/Device and user authorization requirements.
The resource server can decide on what user properties are required to allow access. 

## Error Responses 

If the access token is valid but the user authentication methods attached to the device is missing for the request, an HTTP 403 or a 404 is returned.

"error": : REQUIRED. A single ASCII {{USASCII}} error code from the following:

~~~
 "invalid_request":
 :     The request is missing a required parameter, includes an
       unsupported parameter value,
       repeats a parameter, includes multiple credentials,
       utilizes more than one mechanism for authenticating the
       client, or is otherwise malformed.

 "invalid_client":
 :     Client authentication failed (e.g., unknown client, no
       client authentication included, or unsupported
       authentication method).  The authorization server MAY
       return an HTTP 401 (Unauthorized) status code to indicate
       which HTTP authentication schemes are supported.  If the
       client attempted to authenticate via the `Authorization`
       request header field, the authorization server MUST
       respond with an HTTP 401 (Unauthorized) status code and
       include the `WWW-Authenticate` response header field
       matching the authentication scheme used by the client.

 "invalid_session":
 :     The provided `auth_session` is
       invalid, expired, revoked, or is otherwise invalid.

 "invalid_scope":
 :     The requested scope is invalid, unknown, malformed, or
       exceeds the scope granted by the resource owner.

 "insufficient_authorization":
 :     The presented authorization is insufficient, and the authorization
       server is requesting the client take additional steps to
       complete the authorization.

 Values for the `error` parameter MUST NOT include characters
 outside the set %x20-21 / %x23-5B / %x5D-7E.

 The authorization server MAY extend these error codes with custom
 messages based on the requirements of the authorization server.
 ~~~ 

# Security Considerations {#security-considerations}

## Phishing

Malicious applications can be used to implement a phishing attack.

## Auth Session {#auth-session-security}

An access token for the auth_session can always be used and any Application/Device can request a new secure session. While this session is secure, the Application/Device has no specific user authentication methods. The Application/Device is responsible for implementing this and the user properties can be attached using a secure API. 

### Auth Session Application/Device Binding ands DPoP

The auth_session is attached to the Application/Device key not the DPoP key. The public/private key used for DPoP SHOULD use be a separate key and can be used in a standard way.

Application/Device binding of the auth_session value ensures that the context referenced by the auth_session cannot be stolen and reused by another device.
