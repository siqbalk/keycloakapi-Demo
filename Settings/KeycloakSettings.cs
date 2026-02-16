namespace KeyCloakDemoApi.Settings
{
    public class KeycloakSettings
    {
        public string Authority { get; set; }
        public string ValidIssuer { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public SwaggerSettings Swagger { get; set; }
    }
}
