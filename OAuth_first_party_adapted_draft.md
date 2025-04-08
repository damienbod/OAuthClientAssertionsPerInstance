## Initial Authorization Request

~~~ ascii-art
                                                +-------------------+
                                                |   Authorization   |
                          (A) Device            |      Server       |
                              Registration      |                   |
                              Request           |+-----------------+|
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

- (A) The first-party client starts the flow and creates an asymmetric private, public key pair. The client initiates the authorization request by making a POST request to the Device Registration Endpoint using the public key.
- (B) The Authorization Server determines whether the information provided to the Device Registration Endpoint is sufficient. The server creates an 'auth_session' for the public key and returns the 'auth_session' in the response.
- (C) The Device requests an access token with a client assertion and OAuth client credentials created using the private key. The 'auth_session' is added to the client assertion using the 'device_auth_session' claim. The public key attached to the auth_session is used to validate the client assertion. Optional, DPoP is used to request the token. DPoP does not use the same private, public key pair.
- (D) The Authorization Server returns an access token from the Token Endpoint. The 'auth_session' is returned in the access token. 
- (E) The Authorization Challenge Endpoint is used to attach user authentication properties to the device and the auth_session. The Authorization Server validates the access token using the auth_session and the device public key.
- (F) ..
- (G) repeat for n-user properties
- (H) ..
