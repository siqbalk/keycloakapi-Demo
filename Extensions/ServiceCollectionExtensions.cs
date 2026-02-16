using KeyCloakDemoApi.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace KeyCloakDemoApi.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddAuthenticationAndAuthorization(
     this IServiceCollection services,
     IConfiguration configuration)
        {
            var keycloak = configuration.GetSection("Keycloak").Get<KeycloakSettings>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = keycloak.Authority;
                    options.RequireHttpsMetadata = keycloak.RequireHttpsMetadata;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = true,
                        ValidIssuer = keycloak.ValidIssuer,
                        RoleClaimType = "realm_access.roles"
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
            });

            return services;
        }

        public static IServiceCollection AddSwagger(
    this IServiceCollection services,
    IConfiguration configuration)
        {
            var keycloak = configuration.GetSection("Keycloak").Get<KeycloakSettings>();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "KeycloakDemo API", Version = "v1" });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(keycloak.Swagger.AuthorizationUrl),
                            TokenUrl = new Uri(keycloak.Swagger.TokenUrl),
                            Scopes = new Dictionary<string, string>
                    {
                        { keycloak.Swagger.Scope, "OpenID Connect scope" }
                    }
                        }
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                },
                new[] { keycloak.Swagger.Scope }
            }
        });
            });

            return services;
        }




    }
}

