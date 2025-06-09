# Experimental alternative flow for OAuth 2.0 First-Party Applications

Looks at an alternative way of implementing a native app authentication and authorization. At present, a web browser is used to implement authentication of native applications when using OAuth and OpenID Connect. The flow demonstrated in this repository is based on the OAuth 2.0 for First-Party Applications draft and adapted to be focused on the device.

Related issue: https://github.com/oauth-wg/oauth-first-party-apps/issues/135

Draft flow docs: [Device first OAuth 2.0 First-Party Applications](/OAuth_first_party_adapted_draft.md)

[![.NET](https://github.com/damienbod/OAuthClientAssertionsPerInstance/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/OAuthClientAssertionsPerInstance/actions/workflows/dotnet.yml)

Blog: [Experimental alternative flow for OAuth First-Party Applications](https://damienbod.com)

## OAuth 2.0 Client assertions using Duende IdentityServer

Use the **ICustomTokenRequestValidator** interface

## Duende.IdentityServer.Validation.PrivateKeyJwtSecretValidator

Validates JWTs that are signed with either X.509 certificates or keys wrapped in a JWK. Can be enabled by calling the AddJwtBearerClientAuthentication DI extension method.

https://github.com/DuendeArchive/IdentityServer4/blob/archive/src/IdentityServer4/src/Validation/Default/PrivateKeyJwtSecretValidator.cs

## Client assertion, Private Key JWTs

```csharp
// This is the IdentityServer method
    public static IIdentityServerBuilder AddJwtBearerClientAuthentication(this IIdentityServerBuilder builder)
    {
        builder.AddSecretParser<JwtBearerClientAssertionSecretParser>();
        builder.AddSecretValidator<PrivateKeyJwtSecretValidator>();

        return builder;
    }

// So do this instead of a call to AddJwtBearerClientAuthentication
builder.AddSecretParser<JwtBearerClientAssertionSecretParser>();
builder.AddSecretValidator<YourSecretValidator>(); // TODO, create your secret validator class
```

## Migrations

```
Add-Migration "InitializeApp" -Context ApplicationDbContext
```

```
Update-Database -Context ApplicationDbContext
```

## Notes

The **DefaultTokenCreationService** can be used to add custom claims to the token

```
public class CustomTokenCreationService : DefaultTokenCreationService
{
    public CustomTokenCreationService(IClock clock,
        IKeyMaterialService keys,
        IdentityServerOptions options,
        ILogger<DefaultTokenCreationService> logger)
        : base(clock, keys, options, logger)
    {
    }

    protected override Task<string> CreatePayloadAsync(Token token)
    {
        token.Audiences.Add("custom1");
        return base.CreatePayloadAsync(token);
    }
}
```

## History

- 2025-06-07 Updated packages, added build workflow, clean up
- 2025-04-13 Initial version

## Links

https://docs.duendesoftware.com/identityserver/v7/tokens/authentication/jwt/

https://docs.duendesoftware.com/identityserver/v7/reference/validators/custom_token_request_validator/

https://docs.duendesoftware.com/identityserver/v7/tokens/authentication/jwt/

https://docs.duendesoftware.com/foss/accesstokenmanagement/advanced/client_assertions/

https://www.scottbrady.io/oauth/removing-shared-secrets-for-oauth-client-authentication

## Specs

https://www.rfc-editor.org/rfc/rfc7636

https://datatracker.ietf.org/doc/draft-ietf-oauth-first-party-apps

https://github.com/oauth-wg/oauth-first-party-apps

https://github.com/oauth-wg/oauth-first-party-apps/blob/main/draft-ietf-oauth-first-party-apps.md

https://datatracker.ietf.org/doc/html/rfc9449