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


## Device Registration Request

~~~
POST /token HTTP/1.1
Host: as.example.com
...
client_id=cid_235saw4r4
&grant_type=fp_register
&public_key=<public_key>
&state=<state>
&nonce=<nonce>
 ~~~

## Device Registration Response

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

Example of FP Token

~~~
{
    "alg": "RS256",
    "kid": "9E08135FAEFB9D9E7F7520792656BA0A",
    "typ": "fp+jwt"
}.{
    "iss": "https://localhost:5101",
    "nbf": 1744120238,
    "iat": 1744120238,
    "aud": "<client_id>"
    "exp": 1744123838,
    "auth_session": "AC7E69B69D627CDDA61AF41518B046E1",
    "nonce": "<nonce>"
}
~~~

## Token Endpoint Client Credentials token request using client assertion Request

The Authorization Server uses the 'device_auth_session' to find the correct public key to authenticate the client assertion. The value is added to the access token on a successful validation. 

~~~
{
  "aud": "https://localhost:5101/connect/token",
  "iss": "onboarding-user-client",
  "exp": 1744142406,
  "jti": "668723c5-7324-4879-a780-83c1edf2232d",
  "sub": "onboarding-user-client",
  "iat": 1744142346,
  "device_auth_session": "E2524405BA1EF4A956CA2B12000F7BFC",
  "nbf": 1744142346
}
~~~

## Token Endpoint Successful Response

This specification extends the OAuth 2.0 [RFC6749] token response
defined in Section 5.1 with the additional parameter auth_session

An example successful token response is below:

~~~
HTTP/1.1 200 OK
Content-Type: application/json
Cache-Control: no-store

{
    "access_token": "2YotnFZFEjr1zCsicMWpAA",
    "token_type": "Bearer",
    "expires_in": 3600,
    "refresh_token": "tGzv3JOkF0XG5Qx2TlKWIA",
}
~~~

### Example of Access Token using DPoP

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
        "auth_session:AC7E69B69D627CDDA61AF41518B046E1",
        "OnboardingUserScope"
    ],
    "client_id": "onboarding-user-client",
    "jti": "7651BD4201E947DA4220A01D7207F44E"
}
~~~

## Authorization Challenge Request

The request body can be anything depending on the application authorization requirements.

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

## Using the access token on a resource server

The auth_session is included inthe access token. This claim is used to implement any further device/user authorization requirements.
The resource server can decide on what user properties are required to allow access. 

## Error Responses

If the access token is valid but the user authentication methods attached to the device is missing for the request, an HTTP 403 or a 404 is returned.
