using _15_Integration.Tests.Helpers;
using API.Resources;
using IdentityServer;
using Integration.Tests.TestFixture;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Tests
{
    public class IntTests
    {
        private readonly HttpClient apiClient;
        private readonly HttpClient identityClient;
        private string access_token;

        public IntTests()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            apiClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost:5001") };
            identityClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost:5005") };
        }

        [OneTimeSetUp]
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

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [Test]
        [TestCase("TestProject", "Long description", "ShortDescription", "https://testuri.com/", null, 0, null, false, null, HttpStatusCode.Created)]
        [TestCase("", "", "", "" , null, null, null, false, null, HttpStatusCode.Forbidden)]
        public async Task PostProject(string name, string description, string shortdescription, string uri, ICollection<CollaboratorResource> collaborators, int fileId, CallToActionResource callToAction, bool institutePrivate, ICollection<ProjectCategoryResource> categories, HttpStatusCode expectedStatusCode)
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
                InstitutePrivate = institutePrivate,
                Categories = categories
            };

            apiClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", access_token);
            apiClient.DefaultRequestHeaders.Add("IdentityId", "88421113");

            HttpContent c = new StringContent(JsonConvert.SerializeObject(projectResource), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await apiClient.PostAsync("/api/Project", c);


            Assert.AreEqual(expectedStatusCode, response.StatusCode);
        }
    }
}
