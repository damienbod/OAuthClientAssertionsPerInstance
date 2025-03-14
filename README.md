# OAUTH Client assertions per instance

## Dynamic Client Registration

## Client assertion, Private Key JWTs

## Migrations

```
Add-Migration "InitializeAppUsers" -c ApplicationDbContext

Add-Migration "InitializeAppConfigurations" -c ConfigurationDbContext
```

```
Update-Database -Context ApplicationDbContext

Update-Database -Context ConfigurationDbContext
```

## Links

https://docs.duendesoftware.com/identityserver/v7/configuration/dcr/

https://docs.duendesoftware.com/identityserver/v7/tokens/authentication/jwt/