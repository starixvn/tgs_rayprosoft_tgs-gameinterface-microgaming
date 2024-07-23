using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest;

public class GameApiClient_WalletTranscationsDetailsUnitTest
{
	[Fact]
	public async Task TranscationExist()
	{
		// Arrange
		var stsApiBaseUrl = new Uri("https://sts-demo.k2net.io");
		var clientId = "abc123";
		var clientSecret = "01J2WPK2XEN8CK2MR2PB5N39JX";
		var apiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1");
		var agentCode = "01J2X89FF0E6C5SDXQWF6J8SM7";
		var walletTransactionId = "01J2XEENW5KEMWF83K8WQPDJMY";

		var handler = new MockHttpMessageHandler();
		handler
			.Expect(HttpMethod.Get, new Uri(apiBaseUrl, $"/agents/{agentCode}/WalletTransactions/{walletTransactionId}").ToString())
			.WithHeaders(new Dictionary<string, string>
			{
				["Authorization"] = "Bearer 01J2X8HBTR42XAH5NAPTRSDPHQ",
			})
			.Respond(HttpStatusCode.OK, JsonContent.Create(JsonNode.Parse("""
		{
			"details": [
				{
					"product": "SMG",
					"status": "Succeeded",
					"amount": 100.0000
				}
			],
			"idempotencyKey": "e0cf53a1-5a95-47ff-b661-a1ba44db9fde",
			"id": "031D105D0199430B0001000000002F04B6D0",
			"status": "Succeeded",
			"type": "Deposit",
			"playerId": "playerId",
			"amount": 100.0000,
			"externalTransactionId": "425079f4-c9cf-4739-ba8e-7a53a25c2ed2TEST",
			"createdDateUTC": "2022-02-16T06:21:16.189",
			"uri": "https://api-demo.k2net.io/api/v1/agents/DemoAgent/WalletTransactions/031D105D0199430B0001000000002F04B6D0"
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
		var actual = await sut
			.WalletTransactionsDetailsAsync(
				stsApiBaseUrl: stsApiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				apiBaseUrl: apiBaseUrl,
				agentCode: agentCode,
				walletTransactionId: walletTransactionId,
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.True(actual);
	}

	[Fact]
	public async Task TranscationNotExist()
	{
		// Arrange
		var stsApiBaseUrl = new Uri("https://sts-demo.k2net.io");
		var clientId = "abc123";
		var clientSecret = "01J2WPK2XEN8CK2MR2PB5N39JX";
		var apiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1");
		var agentCode = "01J2X89FF0E6C5SDXQWF6J8SM7";
		var walletTransactionId = "01J2XEENW5KEMWF83K8WQPDJMY";

		var handler = new MockHttpMessageHandler();
		handler
			.Expect(HttpMethod.Get, new Uri(apiBaseUrl, $"/agents/{agentCode}/WalletTransactions/{walletTransactionId}").ToString())
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
		var fakerDateTimeConverter = Substitute.For<IDateTimeConverter>();
		var fakerLogger = NullLogger<GameApiClient>.Instance;
		var sut = new GameApiClient(fakerLogger, fakerTokenRepo, fakerDateTimeConverter, fakerHttpClient);

		// Act
		var actual = await sut
			.WalletTransactionsDetailsAsync(
				stsApiBaseUrl: stsApiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				apiBaseUrl: apiBaseUrl,
				agentCode: agentCode,
				walletTransactionId: walletTransactionId,
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.False(actual);
	}
}
