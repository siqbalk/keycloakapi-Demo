using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace KeyCloakDemoApi.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://keycloakapp.whitewater-36b706c9.centralindia.azurecontainerapps.io/realms/keyclock-demo";
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = "https://keycloakapp.whitewater-36b706c9.centralindia.azurecontainerapps.io/realms/keyclock-demo",
                    RoleClaimType = "realm_access.roles"
                };
            });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("UserPolicy", policy =>
                    policy.RequireRole("User"));
            });

            return services;
        }


        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "KeycloakDemo API", Version = "v1" });

                // Define OAuth2 for Keycloak

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://keycloakapp.whitewater-36b706c9.centralindia.azurecontainerapps.io/realms/keyclock-demo/protocol/openid-connect/auth"),
                            TokenUrl = new Uri("http://https://keycloakapp.whitewater-36b706c9.centralindia.azurecontainerapps.io/realms/keyclock-demo/protocol/openid-connect/token"),
                            Scopes = new Dictionary<string, string>
           {
                { "openid", "OpenID Connect scope" }
           }
                        }
                    }
                });

                // Apply security globally
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
                    new[] { "openid" }
                }
           });
            });

            return services;
        }
    }
}

