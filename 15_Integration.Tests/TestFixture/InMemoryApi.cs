using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Integration.Tests.TestFixture
{
    public class InMemoryApi
    {
        public readonly HttpClient Client;
        public readonly TestServer API;

        public InMemoryApi()
        {
            Assembly startupAssembly = typeof(API.Startup).GetTypeInfo().Assembly;
            string contentRoot = GetProjectPath(Path.Combine(""), startupAssembly);

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("appsettingsapi.Development.json");

            IWebHostBuilder builder = new WebHostBuilder()
            .UseEnvironment("Development")
            .UseConfiguration(configurationBuilder.Build())
            .UseStartup<API.Startup>().UseSetting("testing", "true")
            .UseContentRoot(contentRoot)
            .UseSentry()
            .UseKestrel(o => o.AddServerHeader = false);

            API = new TestServer(builder);
            Client = API.CreateClient();
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
                    if(new FileInfo(Path.Combine(projectDirectoryInfo.FullName, "API", $"{projectName}.csproj")).Exists)
                        return Path.Combine(projectDirectoryInfo.FullName, "API");
            }
            while(directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }

    }
}
