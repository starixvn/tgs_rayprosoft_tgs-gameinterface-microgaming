using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest;

public class GameApiClient_PlayersUnitTest
{
	[Fact]
	public async Task CreateNew()
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
			.Expect(HttpMethod.Post, new Uri(apiBaseUrl, $"/agents/{agentCode}/players").ToString())
			.WithHeaders(new Dictionary<string, string>
			{
				["Authorization"] = "Bearer 01J2X8HBTR42XAH5NAPTRSDPHQ",
			})
			.WithExactFormData(new Dictionary<string, string>
			{
				["playerId"] = "4ucmqplxb029",
			})
			.Respond(HttpStatusCode.Created, JsonContent.Create(JsonNode.Parse("""
			{
				"playerId": "4ucmqplxb029",
				"createdDateUTC": "2021-12-30T08:34:32.809",
				"isLocked": false,
				"uri": "https://api-demo.k2net.io/api/v1/agents/DemoAgent/Players/4ucmqplxb029"
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
		var fakerDateTimeConverter = Substitute.For<IDateTimeConverter>();
		var fakerLogger = NullLogger<GameApiClient>.Instance;
		var sut = new GameApiClient(fakerLogger, fakerTokenRepo, fakerDateTimeConverter, fakerHttpClient);

		// Act
		await sut
			.PlayersAsync(
				stsApiBaseUrl: stsApiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				apiBaseUrl: apiBaseUrl,
				agentCode: agentCode,
				playerId: playerId,
				cancellationToken: default)
			.ConfigureAwait(true);
	}

	[Fact]
	public async Task CreateExist()
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
			.Expect(HttpMethod.Post, new Uri(apiBaseUrl, $"/agents/{agentCode}/players").ToString())
			.WithHeaders(new Dictionary<string, string>
			{
				["Authorization"] = "Bearer 01J2X8HBTR42XAH5NAPTRSDPHQ",
			})
			.WithExactFormData(new Dictionary<string, string>
			{
				["playerId"] = "4ucmqplxb029",
			})
			.Respond(HttpStatusCode.OK, JsonContent.Create(JsonNode.Parse("""
			{
				"playerId": "4ucmqplxb029",
				"createdDateUTC": "2021-12-30T08:34:32.809",
				"isLocked": false,
				"uri": "https://api-demo.k2net.io/api/v1/agents/DemoAgent/Players/4ucmqplxb029"
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
		var fakerDateTimeConverter = Substitute.For<IDateTimeConverter>();
		var fakerLogger = NullLogger<GameApiClient>.Instance;
		var sut = new GameApiClient(fakerLogger, fakerTokenRepo, fakerDateTimeConverter, fakerHttpClient);

		// Act
		await sut
			.PlayersAsync(
				stsApiBaseUrl: stsApiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				apiBaseUrl: apiBaseUrl,
				agentCode: agentCode,
				playerId: playerId,
				cancellationToken: default)
			.ConfigureAwait(true);
	}
}
