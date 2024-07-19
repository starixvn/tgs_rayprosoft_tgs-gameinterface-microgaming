using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest;

public class GameApiClient_PlayersDetailsUnitTest
{
	[Fact]
	public async Task PlayerExist()
	{
		// Arrange
		var stsApiBaseUrl = new Uri("https://sts-demo.k2net.io");
		var clientId = "abc123";
		var clientSecret = "01J2WPK2XEN8CK2MR2PB5N39JX";
		var apiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1");
		var agentCode = "01J2X89FF0E6C5SDXQWF6J8SM7";
		var playerId = "4ucmqplxb029";

		var handler = new MockHttpMessageHandler();
		handler
			.Expect(HttpMethod.Get, new Uri(apiBaseUrl, $"/agents/{agentCode}/players/{playerId}").ToString())
			.WithHeaders(new Dictionary<string, string>
			{
				["Authorization"] = "Bearer 01J2X8HBTR42XAH5NAPTRSDPHQ",
			})
			.Respond(HttpStatusCode.OK, JsonContent.Create(JsonNode.Parse("""
			{
				"isLocked": false,
				"playerId": "4ucmqplxb029",
				"createdDateUTC": "2024-07-16T16:21:16.189",
				"balance": {
					"total": 100,
					"isPartial": false,
					"details": {
						"walletBalance": {
							"total": 100
						},
						"productsBalance": {
							"total": 0,
							"details": []
						}
					}
				},
				"uri": "https://xxx.xxx.xxx/xxx"
			}
			""")));

		var fakerHttpClient = handler.ToHttpClient();
		var fakerTokenRepo = Substitute.For<IStsTokenRepository>();
		fakerTokenRepo
			.GetTokenAsync(
				apiBaseUrl: stsApiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				cancellationToken: default)
			.Returns("01J2X8HBTR42XAH5NAPTRSDPHQ");
		var fakerLogger = NullLogger<GameApiClient>.Instance;
		var sut = new GameApiClient(fakerLogger, fakerTokenRepo, fakerHttpClient);

		// Act
		var actual = await sut
			.PlayersDetailsAsync(
				stsApiBaseUrl: stsApiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				apiBaseUrl: apiBaseUrl,
				agentCode: agentCode,
				playerId: playerId,
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.True(actual.isPlayerExist);
		Assert.Equal(100m, actual.balance);
	}

	[Fact]
	public async Task PlayerNotExist()
	{
		// Arrange
		var stsApiBaseUrl = new Uri("https://sts-demo.k2net.io");
		var clientId = "abc123";
		var clientSecret = "01J2WPK2XEN8CK2MR2PB5N39JX";
		var apiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1");
		var agentCode = "01J2X89FF0E6C5SDXQWF6J8SM7";
		var playerId = "4ucmqplxb029";

		var handler = new MockHttpMessageHandler();
		handler
			.Expect(HttpMethod.Get, new Uri(apiBaseUrl, $"/agents/{agentCode}/players/{playerId}").ToString())
			.WithHeaders(new Dictionary<string, string>
			{
				["Authorization"] = "Bearer 01J2X8HBTR42XAH5NAPTRSDPHQ",
			})
			.Respond(HttpStatusCode.NotFound);

		var fakerHttpClient = handler.ToHttpClient();
		var fakerTokenRepo = Substitute.For<IStsTokenRepository>();
		fakerTokenRepo
			.GetTokenAsync(
				apiBaseUrl: stsApiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				cancellationToken: default)
			.Returns("01J2X8HBTR42XAH5NAPTRSDPHQ");
		var fakerLogger = NullLogger<GameApiClient>.Instance;
		var sut = new GameApiClient(fakerLogger, fakerTokenRepo, fakerHttpClient);

		// Act
		var actual = await sut
			.PlayersDetailsAsync(
				stsApiBaseUrl: stsApiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				apiBaseUrl: apiBaseUrl,
				agentCode: agentCode,
				playerId: playerId,
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.False(actual.isPlayerExist);
		Assert.Equal(0m, actual.balance);
	}
}
