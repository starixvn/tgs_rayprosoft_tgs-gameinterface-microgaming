using NSubstitute;
using Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest.ApplicationServices;

public class PlayerCreateCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var query = new PlayerCreateCommand
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			PlayerCode = "4ucmqplxb029",
		};
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		var sut = new PlayerCreateCommandService(fakeGameApiClient);

		// Act
		await sut.ExecuteAsync(query).ConfigureAwait(true);

		// Assert
		_ = fakeGameApiClient
			.Received(1)
			.PlayersAsync(
				stsApiBaseUrl: Arg.Is(query.StsApiBaseUrl),
				clientId: Arg.Is(query.ClientId),
				clientSecret: Arg.Is(query.ClientSecret),
				apiBaseUrl: Arg.Is(query.ApiBaseUrl),
				agentCode: Arg.Is(query.AgentCode),
				playerId: Arg.Is(query.PlayerCode),
				cancellationToken: Arg.Any<CancellationToken>());
	}
}
