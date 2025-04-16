using Duende.AspNetCore.Authentication.JwtBearer.DPoP;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Idp.Data;
using Idp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Serilog;

namespace Idp;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        builder.Services.AddTransient<PublicKeyService>();
        builder.Services.AddScoped<OnboardingUserService>();
        builder.Services.AddScoped<IScopeParser, ParameterizedScopeParser>();
        builder.Services.AddTransient<ITokenCreationService, CustomTokenCreationService>();

        builder.Services.AddRazorPages();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var idsvrBuilder = builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients(builder.Environment))
            .AddAspNetIdentity<ApplicationUser>();

        idsvrBuilder.AddSecretParser<JwtBearerClientAssertionSecretParser>();
        idsvrBuilder.AddSecretValidator<PerInstancePrivateKeyJwtSecretValidator>();

        var stsServer = builder.Configuration["StsServer"];

        builder.Services.AddAuthentication()
            .AddJwtBearer("onboardinguser", options =>
            {
                options.Authority = stsServer;
                // TODO add valid aud
                options.TokenValidationParameters.ValidateAudience = false;
                options.MapInboundClaims = false;

                options.TokenValidationParameters.ValidTypes = ["at+jwt"];
            });

        // layers DPoP onto the "token" scheme above
        builder.Services.ConfigureDPoPTokensForScheme("onboardinguser", opt =>
        {
            // Chose a validation mode: either Nonce or IssuedAt. With nonce validation,
            // the api supplies a nonce that must be used to prove that the token was
            // not pre-generated. With IssuedAt validation, the client includes the
            // current time in the proof token, which is compared to the clock. Nonce
            // validation provides protection against some attacks that are possible
            // with IssuedAt validation, at the cost of an additional HTTP request being
            // required each time the API is invoked.
            //
            // See RFC 9449 for more details.
            opt.ValidationMode = ExpirationValidationMode.IssuedAt; // IssuedAt is the default.
        });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("onboardinguserpolicy", policy =>
            {
                policy.RequireClaim("scope", "OnboardingUserScope");
            });
        builder.Services.AddControllers();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        IdentityModelEventSource.ShowPII = true;

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        app.MapControllers()
            .RequireAuthorization();

        return app;
    }
}