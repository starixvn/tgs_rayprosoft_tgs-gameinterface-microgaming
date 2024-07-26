using NSubstitute;
using Tgs.GameInterface.MicroGaming.Core.ApplicationServices;
using Tgs.GameInterface.MicroGaming.Core.Models;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest.ApplicationServices;

public class GameRecordBetLogQueryServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var query = new GameRecordBetLogQuery
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			Limit = 20000,
			StartingAfter = "",
		};
		var item = new BetsResponseItem(
			BetUid: "01J2ZP5RAB77CGDHWNFPFKDRRP",
			CreatedDateUtc: new DateTimeOffset(2024, 07, 17, 17, 30, 00, TimeSpan.Zero),
			GameStartTimeUtc: new DateTimeOffset(2024, 07, 17, 17, 30, 00, TimeSpan.Zero),
			GameEndTimeUtc: new DateTimeOffset(2024, 07, 17, 17, 30, 30, TimeSpan.Zero),
			GameCode: "01J2ZP8X1C2GCX0DEYZC0PKCZG",
			PlayerId: "4ucmqplxb029",
			Currency: "CNY",
			BetAmount: 10m,
			PayoutAmount: 0m,
			BetStatus: BetStatus.Closed,
			ExternalTransactionId: "01J2ZPB6VWD9YSNWWD7JRHH7A3",
			AdditionalData: "01J2ZPBK2WV0F6NE1YCN2PK396");
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		_ = fakeGameApiClient
			.BetsAsync(
				stsApiBaseUrl: Arg.Is(query.StsApiBaseUrl),
				clientId: Arg.Is(query.ClientId),
				clientSecret: Arg.Is(query.ClientSecret),
				apiBaseUrl: Arg.Is(query.ApiBaseUrl),
				agentCode: Arg.Is(query.AgentCode),
				limit: Arg.Is(query.Limit),
				startingAfter: Arg.Is(query.StartingAfter),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new[] { item }.ToAsyncEnumerable());
		var sut = new GameRecordBetLogQueryService(fakeGameApiClient);

		// Act
		var results = sut.QueryAsync(query);

		// Assert
		await foreach (var actual in results)
		{
			Assert.NotNull(actual);
			Assert.Equal("4ucmqplxb029", actual.PlayerCode);
			Assert.Equal(10m, actual.Bet);
			Assert.Equal(0m, actual.Settle);
			Assert.Equal(-10m, actual.WinLoss);
			Assert.Equal(new DateTimeOffset(2024, 07, 17, 17, 30, 00, TimeSpan.Zero), actual.ExternalCreatedOn);
			Assert.Equal(new DateTimeOffset(2024, 07, 17, 17, 30, 30, TimeSpan.Zero), actual.ExternalClosedOn);
			Assert.Equal("01J2ZP5RAB77CGDHWNFPFKDRRP", actual.ExternalBetId);
			Assert.Equal("01J2ZP8X1C2GCX0DEYZC0PKCZG", actual.ExternalGameId);
			Assert.Equal("01J2ZPBK2WV0F6NE1YCN2PK396", actual.AdditionalData);
		}
	}
}
