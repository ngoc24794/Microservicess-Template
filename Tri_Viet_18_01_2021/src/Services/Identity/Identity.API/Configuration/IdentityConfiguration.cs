using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace Identity.API.Configuration
{
    public class IdentityConfiguration
    {
        /// <summary>
        /// Gets the TestUsers.
        /// </summary>
        public static List<TestUser> TestUsers => new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1144",
                    Username = "mukesh",
                    Password = "mukesh",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Mukesh Murugan"),
                        new Claim(JwtClaimTypes.GivenName, "Mukesh"),
                        new Claim(JwtClaimTypes.FamilyName, "Murugan"),
                        new Claim(JwtClaimTypes.WebSite, "http://codewithmukesh.com"),
                    }
                }
        };

        /// <summary>
        /// Gets the IdentityResources.
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

        /// <summary>
        /// Gets the ApiScopes.
        /// </summary>
        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            new ApiScope("myApi.read"),
            new ApiScope("myApi.write"),
        };

        /// <summary>
        /// Gets the ApiResources.
        /// </summary>
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("myApi")
            {
                Scopes = new List<string>{ "myApi.read","myApi.write" },
                ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
            }
        };

        /// <summary>
        /// Gets the Clients.
        /// </summary>
        public static IEnumerable<Client> Clients => new Client[]
        {
            new Client
            {
                ClientId = "cwm.client",
                ClientName = "Client Credentials Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "myApi.read" }
            },
        };
    }
}