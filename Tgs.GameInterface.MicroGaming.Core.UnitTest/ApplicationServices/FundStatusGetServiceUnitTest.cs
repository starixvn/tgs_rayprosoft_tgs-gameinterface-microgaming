using NSubstitute;
using Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest.ApplicationServices;

public class FundStatusGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var query = new FundStatusQuery
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			ExternalOrderId = "01J2ZNF7BH5T3FGSP7A2TGT3WY",
		};
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		_ = fakeGameApiClient
			.WalletTransactionsDetailsAsync(
				stsApiBaseUrl: Arg.Is(query.StsApiBaseUrl),
				clientId: Arg.Is(query.ClientId),
				clientSecret: Arg.Is(query.ClientSecret),
				apiBaseUrl: Arg.Is(query.ApiBaseUrl),
				agentCode: Arg.Is(query.AgentCode),
				walletTransactionId: Arg.Is(query.ExternalOrderId),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);
		var sut = new FundStatusGetService(fakeGameApiClient);

		// Act
		var actual = await sut.GetAsync(query).ConfigureAwait(true);

		// Assert
		Assert.True(actual);
	}
}
