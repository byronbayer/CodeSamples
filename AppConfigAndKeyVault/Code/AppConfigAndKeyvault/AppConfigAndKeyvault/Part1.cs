using System;
using Microsoft.Extensions.Configuration;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Xunit;

namespace AppConfigAndKeyvault
{
    public class Part1
    {
        private string _nonSecretValue = "platform:NonSecretValue";

        [Fact]
        public void GetValueFromAppConfig()
        {
            DefaultAzureCredential defaultAzureCredential = new DefaultAzureCredential(false);
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
