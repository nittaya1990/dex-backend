using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace Integration.Tests.TestFixture
{
    public class InMemoryIdentity
    {
        public readonly TestServer IdentityServer;
        public readonly HttpClient Client;

        public InMemoryIdentity()
        {
            Assembly startupAssembly = typeof(IdentityServer.Startup).GetTypeInfo().Assembly;
            string contentRoot = GetProjectPath(Path.Combine(""), startupAssembly);

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("appsettings.Development.json");

            IWebHostBuilder builder = new WebHostBuilder()
            .UseEnvironment("Development")
            .UseConfiguration(configurationBuilder.Build())
            .UseStartup<IdentityServer.Startup>();

            IdentityServer = new TestServer(builder);
            Client = IdentityServer.CreateClient();
        }

        public static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
        {
            string projectName = startupAssembly.GetName().Name;
            string applicationBasePath = AppContext.BaseDirectory;
            DirectoryInfo directoryInfo = new DirectoryInfo(applicationBasePath);

            do
            {
                directoryInfo = directoryInfo.Parent;

                DirectoryInfo projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));

                if(projectDirectoryInfo.Exists)
                    if(new FileInfo(Path.Combine(projectDirectoryInfo.FullName, "IdentityServer", $"{projectName}.csproj")).Exists)
                        return Path.Combine(projectDirectoryInfo.FullName, "IdentityServer");
            }
            while(directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }
    }
}
