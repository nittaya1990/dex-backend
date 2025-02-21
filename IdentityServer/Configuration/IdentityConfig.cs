/*
* Digital Excellence Copyright (C) 2020 Brend Smits
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU Lesser General Public License as published
* by the Free Software Foundation version 3 of the License.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty
* of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU Lesser General Public License for more details.
*
* You can find a copy of the GNU Lesser General Public License
* along with this program, in the LICENSE.md file in the root project directory.
* If not, see https://www.gnu.org/licenses/lgpl-3.0.txt
*/

using Configuration;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Models.Defaults;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer.Configuration
{

    public static class IdentityConfig
    {

        public static IEnumerable<ApiResource> Apis =>
            new[]
            {
                new ApiResource("dex-api", "Digital Excellence API")
                {
                    Scopes =
                    {
                        new Scope(nameof(Defaults.Scopes.ProjectRead)),
                        new Scope(nameof(Defaults.Scopes.AdminProjectWrite)),
                        new Scope(nameof(Defaults.Scopes.ProjectWrite)),
                        new Scope(nameof(Defaults.Scopes.UserWrite)),
                        new Scope(nameof(Defaults.Scopes.UserRead)),
                        new Scope(nameof(Defaults.Scopes.CategoryWrite)),
                        new Scope(nameof(Defaults.Scopes.CategoryRead)),
                        new Scope(nameof(Defaults.Scopes.HighlightWrite)),
                        new Scope(nameof(Defaults.Scopes.HighlightRead)),
                        new Scope(nameof(Defaults.Scopes.EmbedWrite)),
                        new Scope(nameof(Defaults.Scopes.EmbedRead)),
                        new Scope(nameof(Defaults.Scopes.FileWrite)),
                        new Scope(nameof(Defaults.Scopes.UserTaskWrite)),
                        new Scope(nameof(Defaults.Scopes.WizardPageWrite))
                    }
                },
                new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
                   {
                       new IdentityResources.OpenId(),
                       new IdentityResources.Profile(),
                       new IdentityResources.Email()
                   };
        }

        public static IEnumerable<Client> Clients(Config config)
        {
            return new[]
                   {
                        new Client
                        {
                            ClientId = "integration-test",
                            AllowedGrantTypes = GrantTypes.Code,
                            ClientSecrets =
                            {
                                //new Secret(config.IntegrationTests.ClientSecret.Sha256())
                                new Secret("testtest".Sha256())
                            },
                            AllowedScopes =
                            {
                                nameof(Defaults.Scopes.ProjectRead),
                                nameof(Defaults.Scopes.AdminProjectWrite),
                                nameof(Defaults.Scopes.ProjectWrite),
                                nameof(Defaults.Scopes.UserWrite),
                                nameof(Defaults.Scopes.UserRead),
                                nameof(Defaults.Scopes.CategoryWrite),
                                nameof(Defaults.Scopes.CategoryRead),
                                nameof(Defaults.Scopes.HighlightRead),
                                nameof(Defaults.Scopes.HighlightWrite),
                                nameof(Defaults.Scopes.EmbedWrite),
                                nameof(Defaults.Scopes.EmbedRead),
                                nameof(Defaults.Scopes.FileWrite),
                                nameof(Defaults.Scopes.UserTaskWrite),
                                nameof(Defaults.Scopes.WizardPageWrite)
                            }
                            //,
                            //Claims = new List<Claim>
                            //        {
                            //            new Claim(JwtClaimTypes.Role, Defaults.Roles.BackendApplication)
                            //        }
                        },
                       // machine to machine client (API -> Identity)
                       new Client
                       {
                           ClientId = "dex-api",
                           AllowedGrantTypes = GrantTypes.ClientCredentials,
                           ClientSecrets =
                           {
                               new Secret(config.ApiAuthentication.ClientSecret.Sha256())
                           },
                           AllowedScopes =
                           {
                               IdentityServerConstants.LocalApi.ScopeName
                           },
                           Claims = new List<Claim>
                                    {
                                        new Claim(JwtClaimTypes.Role, Defaults.Roles.BackendApplication)
                                    }
                       },

                       // machine to machine client (Identity -> API)
                       new Client
                       {
                           ClientId = "dex-api-client",
                           AllowedGrantTypes = GrantTypes.ClientCredentials,
                           ClientSecrets =
                           {
                               new Secret(config.Api.ClientSecret.Sha256())
                           },
                           AllowedScopes =
                           {
                               nameof(Defaults.Scopes.ProjectRead),
                               nameof(Defaults.Scopes.AdminProjectWrite),
                               nameof(Defaults.Scopes.ProjectWrite),
                               nameof(Defaults.Scopes.UserWrite),
                               nameof(Defaults.Scopes.UserRead),
                               nameof(Defaults.Scopes.CategoryWrite),
                               nameof(Defaults.Scopes.CategoryRead),
                               nameof(Defaults.Scopes.HighlightRead),
                               nameof(Defaults.Scopes.HighlightWrite),
                               nameof(Defaults.Scopes.EmbedWrite),
                               nameof(Defaults.Scopes.EmbedRead),
                               nameof(Defaults.Scopes.FileWrite),
                               nameof(Defaults.Scopes.UserTaskWrite),
                               nameof(Defaults.Scopes.WizardPageWrite)
                           },
                           Claims = new List<Claim>
                                    {
                                        new Claim(JwtClaimTypes.Role, Defaults.Roles.BackendApplication)
                                    }
                       },

                       // interactive ASP.NET Core MVC client
                       new Client
                       {
                           ClientId = "dex-frontend",
                           ClientName = "Digital Excellence Angular Frontend",
                           ClientSecrets =
                           {
                               new Secret(config.Frontend.ClientSecret.Sha256())
                           },
                           AllowedGrantTypes = GrantTypes.Code,
                           RequirePkce = true,
                           RequireClientSecret = false,
                           RequireConsent = false,

                           // where to redirect to after login
                           RedirectUris = new List<string>
                                          {
                                              config.Frontend.RedirectUriFrontend,
                                              config.Frontend.RedirectUriPostman,
                                              config.Frontend.RefreshUriFrontend
                                          },

                           // where to redirect to after logout
                           PostLogoutRedirectUris = new List<string>
                                                    {
                                                        config.Frontend.PostLogoutUriFrontend
                                                    },

                           AllowedScopes = new List<string>
                                           {
                                               IdentityServerConstants.StandardScopes.OpenId,
                                               IdentityServerConstants.StandardScopes.Profile,
                                               IdentityServerConstants.StandardScopes.Email,
                                               "dex-api"
                                           },
                           AllowAccessTokensViaBrowser = true,
                           AllowOfflineAccess = true
                       },
                       new Client
                       {
                           ClientId = "Swagger-UI",
                           ClientName = "Swagger UI",
                           AllowedGrantTypes = GrantTypes.Implicit,
                           AllowAccessTokensViaBrowser = true,
                           AlwaysIncludeUserClaimsInIdToken = true,
                           RedirectUris = new List<string>
                                          {
                                              config.Swagger.RedirectUrisSwagger
                                          },
                           PostLogoutRedirectUris = new List<string>
                                                    {
                                                        config.Swagger.PostLogoutUrisSwagger
                                                    },
                           AllowedScopes = new List<string>
                                           {
                                               IdentityServerConstants.StandardScopes.OpenId,
                                               IdentityServerConstants.StandardScopes.Profile,
                                               "dex-api"
                                           }
                       },
                       new Client
                       {
                           ClientId = config.JobScheduler.ClientId,
                           ClientName = "Digital Excellence Job Scheduler",
                           AllowedGrantTypes = GrantTypes.ClientCredentials,
                           ClientSecrets =
                           {
                               new Secret(config.JobScheduler.ClientSecret.Sha256())
                           },
                           AllowedScopes =
                           {
                               "dex-api"
                           },
                           Claims = new List<Claim>
                                    {
                                        new Claim(JwtClaimTypes.Role, Defaults.Roles.BackendApplication)
                                    }
                       }
                   };
        }

    }

}
