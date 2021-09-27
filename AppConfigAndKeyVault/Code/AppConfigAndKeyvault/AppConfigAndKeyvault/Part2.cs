using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Xunit;

namespace AppConfigAndKeyvault
{
    public class Part2
    {
        private readonly string _nonSecretValue = "platform:NonSecretValue";
        private readonly string _secretValue = "platform:SecretValue";

        public Part2()
        {
            var defaultAzureCredential = new DefaultAzureCredential();
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(options =>
            {
                var connectionString = Constance.ConnectionString;
                options
                    .Select(KeyFilter.Any)
                    .ConfigureKeyVault(kv => kv.SetCredential(new DefaultAzureCredential()))
                    .Connect(connectionString);
            });
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        [Fact]
        public void GetNonSecretFromAppConfig()
        {
            Assert.Equal("This is not a secret", Configuration[_nonSecretValue]);
        }

        [Fact]
        public void GetValueFromAppConfig()
        {
            Assert.Equal("This is a secret", Configuration[_secretValue]);
        }
    }
}