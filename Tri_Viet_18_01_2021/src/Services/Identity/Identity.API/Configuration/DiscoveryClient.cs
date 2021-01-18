/*using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Hosting;

namespace Identity.API.Configuration
{
    public class DiscoveryClient
    {
        private const string DiscoveryEndpoint = "https://server/.well-known/openid-configuration";

        private readonly HttpClient _client;

        public DiscoveryClient()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>();
            var server = new TestServer(builder);

            _client = server.CreateClient();
        }

        [Fact]
        public async Task Discovery_document_should_have_expected_values()
        {
            var doc = await _client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = DiscoveryEndpoint,
                Policy =
                {
                    ValidateIssuerName = false
                }
            });

            // endpoints
            doc.TokenEndpoint.Should().Be("https://server/connect/token");
            doc.AuthorizeEndpoint.Should().Be("https://server/connect/authorize");
            doc.IntrospectionEndpoint.Should().Be("https://server/connect/introspect");
            doc.EndSessionEndpoint.Should().Be("https://server/connect/endsession");

            // jwk
            doc.KeySet.Keys.Count.Should().Be(1);
            doc.KeySet.Keys.First().E.Should().NotBeNull();
            doc.KeySet.Keys.First().N.Should().NotBeNull();
        }
    }
}*/