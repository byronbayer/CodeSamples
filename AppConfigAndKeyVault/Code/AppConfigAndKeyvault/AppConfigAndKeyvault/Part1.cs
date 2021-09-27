using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Xunit;

namespace AppConfigAndKeyvault
{
    public class Part1
    {
        private readonly string _nonSecretValue = "platform:NonSecretValue";

        [Fact]
        public void GetValueFromAppConfig()
        {
            var defaultAzureCredential = new DefaultAzureCredential();
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(options =>
            {
                var connectionString = Constance.ConnectionString;
                options
                    .Select(KeyFilter.Any)
                    .Connect(connectionString);
            });
            var configuration = builder.Build();

            Assert.Equal("This is not a secret", configuration[_nonSecretValue]);
        }
    }
}