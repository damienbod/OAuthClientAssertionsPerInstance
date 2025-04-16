# OAUTH Client assertions using Duende IdentityServer

ICustomTokenRequestValidator 

# Duende.IdentityServer.Validation.PrivateKeyJwtSecretValidator

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

The DefaultTokenCreationService can be used to add custom claims to the token

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

## Links

https://docs.duendesoftware.com/identityserver/v7/tokens/authentication/jwt/

https://docs.duendesoftware.com/identityserver/v7/reference/validators/custom_token_request_validator/

https://docs.duendesoftware.com/identityserver/v7/tokens/authentication/jwt/

https://docs.duendesoftware.com/foss/accesstokenmanagement/advanced/client_assertions/

https://www.scottbrady.io/oauth/removing-shared-secrets-for-oauth-client-authentication

## Specs

https://www.rfc-editor.org/rfc/rfc7636

https://datatracker.ietf.org/doc/draft-ietf-oauth-first-party-apps/