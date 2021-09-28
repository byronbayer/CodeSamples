using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Xunit;

namespace AppConfigAndKeyvault
{
    public class Part3
    {
        
        public Part3()
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(options =>
            {
                var connectionString = Constance.ConnectionString;
                options
                    .Select(KeyFilter.Any)
                    .ConfigureKeyVault(ConfigureKeyVaultOptions)
                    .Connect(connectionString);
            });
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        [Fact]
        public void GetNonSecretFromAppConfig()
        {
            Assert.Equal("This is not a secret", Configuration[Constance.NonSecretValue]);
        }

        [Fact]
        public void GetSecretFromAppConfig()
        {
            Assert.Equal("This is a secret", Configuration[Constance.SecretValue]);
        }

        private static void ConfigureKeyVaultOptions(AzureAppConfigurationKeyVaultOptions kv)
        {
            kv.SetSecretResolver(identifier =>
            {
                string secretValue = null;
                try
                {
                    var secretName = identifier?.Segments?.ElementAtOrDefault(2)?.TrimEnd('/');
                    var secretVersion = identifier?.Segments?.ElementAtOrDefault(3)?.TrimEnd('/');
                    if (identifier is not null)
                    {
                        var azureCredentials = GetAzureCredentials();
                        var secretClient = new SecretClient(new Uri(identifier.GetLeftPart(UriPartial.Authority)),
                            azureCredentials);

                        KeyVaultSecret secret = secretClient.GetSecret(secretName, secretVersion);
                        secretValue = secret?.Value;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    secretValue = "DummyValue";
                }

                return new ValueTask<string>(secretValue);
            });
        }

        private static TokenCredential GetAzureCredentials()
        {
            var isDeployed = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
            var options = new DefaultAzureCredentialOptions
            {
                // Prevent deployed instances from trying things that don't work and generally take too long
                ExcludeInteractiveBrowserCredential = isDeployed,
                ExcludeVisualStudioCodeCredential = isDeployed,
                ExcludeVisualStudioCredential = isDeployed,
                ExcludeSharedTokenCacheCredential = isDeployed,
                ExcludeAzureCliCredential = isDeployed,
                ExcludeManagedIdentityCredential = false,
                Retry =
                {
                    // Reduce retries and timeouts to get faster failures
                    MaxRetries = 2,
                    NetworkTimeout = TimeSpan.FromSeconds(5),
                    MaxDelay = TimeSpan.FromSeconds(5)
                },
                // this helps devs use the right tenant
                InteractiveBrowserTenantId = Constance.DefaultTenantId,
                SharedTokenCacheTenantId = Constance.DefaultTenantId,
                VisualStudioCodeTenantId = Constance.DefaultTenantId,
                VisualStudioTenantId = Constance.DefaultTenantId
            };
            return new DefaultAzureCredential(
                options
            );
        }
    }
}