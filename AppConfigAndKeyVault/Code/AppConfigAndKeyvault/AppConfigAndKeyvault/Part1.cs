using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Xunit;

namespace AppConfigAndKeyvault
{
    public class Part1
    {
        [Fact]
        public void GetValueFromAppConfig()
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(options =>
            {
                options
                    .Select(KeyFilter.Any)
                    .Connect(Constance.ConnectionString);
            });
            var configuration = builder.Build();

            Assert.Equal("This is not a secret", configuration[Constance.NonSecretValue]);
        }
    }
}