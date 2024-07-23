using NSubstitute;
using Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest.ApplicationServices;

public class PlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task PlayerExist()
	{
		// Arrange
		var query = new PlayerBalanceQuery
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			PlayerCode = "4ucmqplxb029",
		};
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		_ = fakeGameApiClient
			.PlayersDetailsAsync(
				stsApiBaseUrl: Arg.Is(query.StsApiBaseUrl),
				clientId: Arg.Is(query.ClientId),
				clientSecret: Arg.Is(query.ClientSecret),
				apiBaseUrl: Arg.Is(query.ApiBaseUrl),
				agentCode: Arg.Is(query.AgentCode),
				playerId: Arg.Is(query.PlayerCode))
			.Returns(ValueTask.FromResult((true, 10m)));
		var sut = new PlayerBalanceGetService(fakeGameApiClient);

		// Act
		var actual = await sut.GetAsync(query).ConfigureAwait(true);

		// Assert
		Assert.True(actual.IsPlayerExist);
		Assert.Equal(10m, actual.Balance);
	}

	[Fact]
	public async Task PlayerNotExist()
	{
		// Arrange
		var query = new PlayerBalanceQuery
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			PlayerCode = "4ucmqplxb029",
		};
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		_ = fakeGameApiClient
			.PlayersDetailsAsync(
				stsApiBaseUrl: Arg.Is(query.StsApiBaseUrl),
				clientId: Arg.Is(query.ClientId),
				clientSecret: Arg.Is(query.ClientSecret),
				apiBaseUrl: Arg.Is(query.ApiBaseUrl),
				agentCode: Arg.Is(query.AgentCode),
				playerId: Arg.Is(query.PlayerCode))
			.Returns(ValueTask.FromResult((false, 0m)));
		var sut = new PlayerBalanceGetService(fakeGameApiClient);

		// Act
		var actual = await sut.GetAsync(query).ConfigureAwait(true);

		// Assert
		Assert.False(actual.IsPlayerExist);
		Assert.Equal(0m, actual.Balance);
	}
}
