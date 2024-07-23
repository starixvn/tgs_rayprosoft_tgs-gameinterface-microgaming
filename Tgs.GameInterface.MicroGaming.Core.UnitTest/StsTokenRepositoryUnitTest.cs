using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest;

public class StsTokenRepositoryUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var apiBaseUrl = new Uri("https://sts-demo.k2net.io");
		var clientId = "abc123";
		var clientSecret = "01J2WPK2XEN8CK2MR2PB5N39JX";

		var handler = new MockHttpMessageHandler();
		_ = handler
			.Expect(HttpMethod.Post, new Uri(apiBaseUrl, "/connect/token").ToString())
			.WithExactFormData(new Dictionary<string, string>
			{
				["client_id"] = clientId,
				["client_secret"] = clientSecret,
				["grant_type"] = "client_credentials",
			})
			.Respond(HttpStatusCode.OK, JsonContent.Create(JsonNode.Parse("""
			{
				"access_token": "01J2WPK9SKTTBJWDXFAP755HEP",
				"expires_in": 3600,
				"token_type": "Bearer"
			}
			""")));
		var fakeHttpClient = handler.ToHttpClient();
		var sut = new StsTokenRepository(fakeHttpClient);

		// Act
		var actual = await sut
			.GetTokenAsync(
				apiBaseUrl: apiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.Equal("01J2WPK9SKTTBJWDXFAP755HEP", actual);
	}
}