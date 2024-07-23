using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest;

public class GameApiClient_BetsUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var stsApiBaseUrl = new Uri("https://sts-demo.k2net.io");
		var clientId = "abc123";
		var clientSecret = "01J2WPK2XEN8CK2MR2PB5N39JX";
		var apiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1");
		var agentCode = "01J2X89FF0E6C5SDXQWF6J8SM7";
		var limit = 20000;
		var startingAfter = "";

		var response = """
		[
			{
				"betUID": "0351406A0199430B000100000006EA15035B",
				"createdDateUTC": "2022-02-16T06:35:40.497",
				"gameStartTimeUTC": "2022-02-16T06:34:37.113",
				"gameEndTimeUTC": "2022-02-16T06:34:37.113",
				"playerId": "4ucmqplxb029",
				"productId": "SMG",
				"productPlayerId": "M0G295682018",
				"platform": "Desktop",
				"gameCode": "SMG_108Heroes",
				"currency": "CNY",
				"betAmount": 3.7500,
				"payoutAmount": 0.0000,
				"betStatus": "Closed",
				"PCA": null,
				"externalTransactionId": "1005639168/720",
				"metadata": {
					"SessionId": "1005639168",
					"TransactionId": "720"
				}
			}
		]
		""";
		var handler = new MockHttpMessageHandler();
		handler
			.Expect(HttpMethod.Get, new Uri(apiBaseUrl, $"/agents/{agentCode}/bets").ToString())
			.WithHeaders(new Dictionary<string, string>
			{
				["Authorization"] = "Bearer 01J2X8HBTR42XAH5NAPTRSDPHQ",
			})
			.WithExactQueryString(new Dictionary<string, string>
			{
				["limit"] = "20000",
				["startingAfter"] = ""
			})
			.Respond(HttpStatusCode.OK, JsonContent.Create(JsonNode.Parse(response)));

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
		fakerDateTimeConverter
			.StringConvertToDatetime(Arg.Any<string>())
			.Returns(str => DateTime.TryParse((string)str[0], out var date) ? new(date, TimeSpan.Zero) : DateTimeOffset.UnixEpoch);
		var fakerLogger = NullLogger<GameApiClient>.Instance;
		var sut = new GameApiClient(fakerLogger, fakerTokenRepo, fakerDateTimeConverter, fakerHttpClient);

		// Act
		var results = sut.BetsAsync(
			stsApiBaseUrl: stsApiBaseUrl,
			clientId: clientId,
			clientSecret: clientSecret,
			apiBaseUrl: apiBaseUrl,
			agentCode: agentCode,
			limit: limit,
			startingAfter: startingAfter,
			cancellationToken: default);

		// Assert
		await foreach (var actual in results)
		{
			Assert.Equal("0351406A0199430B000100000006EA15035B", actual.BetUid);
			Assert.Equal(new DateTimeOffset(2022, 02, 16, 06, 35, 40, 497, TimeSpan.Zero), actual.CreatedDateUtc);
			Assert.Equal(new DateTimeOffset(2022, 02, 16, 06, 34, 37, 113, TimeSpan.Zero), actual.GameStartTimeUtc);
			Assert.Equal(new DateTimeOffset(2022, 02, 16, 06, 34, 37, 113, TimeSpan.Zero), actual.GameEndTimeUtc);
			Assert.Equal("4ucmqplxb029", actual.PlayerId);
			Assert.Equal("CNY", actual.Currency);
			Assert.Equal(3.75m, actual.BetAmount);
			Assert.Equal(0m, actual.PayoutAmount);
			Assert.Equal(BetStatus.Closed, actual.BetStatus);
			Assert.Equal("1005639168/720", actual.ExternalTransactionId);
		}
	}
}
