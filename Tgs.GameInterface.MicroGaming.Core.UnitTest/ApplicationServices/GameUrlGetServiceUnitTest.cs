using NSubstitute;
using Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest.ApplicationServices;

public class GameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var query = new GameUrlQuery
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			PlayerCode = "4ucmqplxb029",
			Language = "zh-CN",
			Platform = GamePlatform.Mobile,
			GameCode = "01J2ZPT69XGTK86YBQ3DS8BGWJ",
		};
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		_ = fakeGameApiClient
			.PlayersSessionsAsync(
				stsApiBaseUrl: Arg.Is(query.StsApiBaseUrl),
				clientId: Arg.Is(query.ClientId),
				clientSecret: Arg.Is(query.ClientSecret),
				apiBaseUrl: Arg.Is(query.ApiBaseUrl),
				agentCode: Arg.Is(query.AgentCode),
				playerId: Arg.Is(query.PlayerCode),
				langCode: Arg.Is(query.Language),
				platform: Arg.Is(query.Platform),
				contentCode: Arg.Is(query.GameCode),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns("https://xxx.xxx.xxx/xxx");
		var sut = new GameUrlGetService(fakeGameApiClient);

		// Act
		var actual = await sut.GetAsync(query).ConfigureAwait(true);

		// Assert
		Assert.Equal("https://xxx.xxx.xxx/xxx", actual);
	}
}
