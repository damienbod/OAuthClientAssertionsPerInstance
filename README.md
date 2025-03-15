# OAUTH Client assertions per instance

ICustomTokenRequestValidator 

## Dynamic Client Registration

## Client assertion, Private Key JWTs

## Migrations

```
Add-Migration "InitializeAppUsers" -c ApplicationDbContext

Add-Migration "InitializeAppConfigurations"
```

```
Update-Database -Context ApplicationDbContext
```

## Links

https://docs.duendesoftware.com/identityserver/v7/configuration/dcr/

https://docs.duendesoftware.com/identityserver/v7/tokens/authentication/jwt/