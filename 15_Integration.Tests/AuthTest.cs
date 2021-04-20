using Integration.Tests.TestFixture;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Tests
{
    public class AuthTest
    {
        private readonly InMemoryFixture inMemoryFixture;
        private readonly HttpClient apiClient;
        private readonly HttpClient identityClient;

        public AuthTest()
        {
            inMemoryFixture = new InMemoryFixture();
            apiClient = inMemoryFixture.InMemoryApi.Client;
            identityClient = inMemoryFixture.InMemoryIdentity.Client;
        }

        [Test]
        public async Task Authorize()
        {
            string tokenUrl = "/connect/token";
            string clientId = "dex-api-client";
            string clientSecret = "Q!P5kqCukQBe77cVk5dqWHqx#8FaC2fDN&bstyxrHtw%5R@3Cz*Z";
            string scope = "ProjectRead ProjectWrite UserRead UserWrite HighlightRead HighlightWrite EmbedRead EmbedWrite";

            Dictionary<string, string> form = new Dictionary<string, string>
                {
                    {"grant_type", "client_credentials"},
                    {"scope", scope } 
                };
            identityClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                                                "Basic", Convert.ToBase64String(
                                                System.Text.ASCIIEncoding.ASCII.GetBytes(
               $"{clientId}:{clientSecret}")));
            HttpResponseMessage response = await identityClient.PostAsync(tokenUrl, new FormUrlEncodedContent(form));
            var jsonContent = await response.Content.ReadAsStringAsync();
            string accessToken = JObject.Parse(jsonContent)["access_token"].ToString();
            Assert.AreNotEqual(null, accessToken);
        }

    }
}
