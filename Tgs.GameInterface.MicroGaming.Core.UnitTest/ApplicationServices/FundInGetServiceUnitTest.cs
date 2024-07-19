using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.MicroGaming.Core.ApplicationServices;
using Tgs.GameInterface.MicroGaming.Core.Models;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest.ApplicationServices;

public class FundInGetServiceTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var query = new FundInQuery
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			PlayerCode = "4ucmqplxb029",
			Amount = 4m,
			OrderId = "01J2ZK59KSJJEQ2VQ06QCC7QEA",
		};
		var json = new JsonObject
		{
			["status"] = "Succeeded",
			["id"] = "01J2ZK8MK301WY3Q9PT15Z4RNV",
			["externalTransactionId"] = "01J2ZK8R7AW3WXD7EPTMRB0FN5",
			["amount"] = 16m
		};
		var response = new WalletTransactionsResponse(
			Id: "01J2ZK8MK301WY3Q9PT15Z4RNV",
			ExternalTransactionId: "01J2ZK8R7AW3WXD7EPTMRB0FN5",
			Amount: 16m,
			Resp: json.ToJsonString());
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		_ = fakeGameApiClient
			.WalletTransactionsAsync(
				stsApiBaseUrl: Arg.Is(query.StsApiBaseUrl),
				clientId: Arg.Is(query.ClientId),
				clientSecret: Arg.Is(query.ClientSecret),
				apiBaseUrl: Arg.Is(query.ApiBaseUrl),
				agentCode: Arg.Is(query.AgentCode),
				playerId: Arg.Is(query.PlayerCode),
				type: Arg.Is("Withdraw"),
				amount: Arg.Is(query.Amount),
				externalTransactionId: Arg.Is(query.OrderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(response));
		var sut = new FundInGetService(fakeGameApiClient);

		// Act
		var actual = await sut.GetAsync(query).ConfigureAwait(true);

		// Assert
		Assert.NotNull(actual);
		Assert.Equal("01J2ZK8R7AW3WXD7EPTMRB0FN5", actual.OrderId);
		Assert.Equal("01J2ZK8MK301WY3Q9PT15Z4RNV", actual.ExternalOrderId);
		Assert.Equal(json.ToJsonString(), actual.Resp);
	}
}
