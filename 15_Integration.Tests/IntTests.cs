using _15_Integration.Tests.Helpers;
using API.Resources;
using IdentityServer;
using Integration.Tests.TestFixture;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Integration.Tests
{
    public class IntTests
    {
        private readonly InMemoryFixture inMemoryFixture;
        private readonly HttpClient apiClient;
        private readonly HttpClient identityClient;

        private string access_token;

        public IntTests()
        {
            inMemoryFixture = new InMemoryFixture();
            apiClient = inMemoryFixture.InMemoryApi.Client;
            identityClient = inMemoryFixture.InMemoryIdentity.Client;
        }

        [SetUp]
        public void Setup()
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
                                                System.Text.Encoding.ASCII.GetBytes(
               $"{clientId}:{clientSecret}")));

            HttpResponseMessage response =
                AsyncHelper.RunSync(() => identityClient.PostAsync(tokenUrl, new FormUrlEncodedContent(form)));
            string jsonContent =
                AsyncHelper.RunSync(() => response.Content.ReadAsStringAsync());

            access_token = JObject.Parse(jsonContent)["access_token"].ToString();
        }

        [Test]
        [TestCase(30)]
        public async Task GetProjects(int expectedAmountOfProjects)
        {         
            HttpResponseMessage response = await apiClient.GetAsync("/api/Project");
            
            ProjectResultsResource projects = JsonConvert.DeserializeObject<ProjectResultsResource>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(projects.Count, expectedAmountOfProjects);
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
            Assert.Fail();
        }

        [Test]
        [TestCase("TestProject", "Long description", "ShortDescription", "https://testuri.com/", null, 0, null, false, null)]
        public async Task PostProject(string name, string description, string shortdescription, string uri, ICollection<CollaboratorResource> collaborators, int fileId, CallToActionResource callToAction, bool institutePrivate, ICollection<ProjectCategoryResource> categories)
        {
            ProjectResource projectResource = new ProjectResource()
            {
                Name = name,
                Description = description,
                ShortDescription = shortdescription,
                Uri = uri,
                Collaborators = collaborators,
                FileId = fileId,
                CallToAction = callToAction,
                InstitutePrivate = false,
                Categories = categories
            };



        }
    }
}
