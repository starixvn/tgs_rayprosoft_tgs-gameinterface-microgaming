using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Tgs.GameInterface.MicroGaming.Client;
using Tgs.GameInterface.MicroGaming.Core;
using Tgs.GameInterface.MicroGaming.Core.ApplicationServices;
using Tgs.GameInterface.MicroGaming.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.MicroGaming.IntegrationTest;

public class PlayerBalanceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var serviceRequest = new PlayerBalanceQuery
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			PlayerCode = "4ucmqplxb029",
		};
		var fakerService = Substitute.For<IGetService<PlayerBalanceQuery, PlayerBalanceResult>>();
		_ = fakerService
			.GetAsync(
				query: Arg.Is(serviceRequest),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(new PlayerBalanceResult(
				IsPlayerExist: true,
				Balance: 10m)));
		var factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
			{
				services.Replace(new(typeof(IGameApiClient), Substitute.For<IGameApiClient>()));
				services.Replace(new(typeof(IGetService<PlayerBalanceQuery, PlayerBalanceResult>), fakerService));
			}));
		var options = new GrpcChannelOptions { HttpHandler = factory.Server.CreateHandler() };
		var channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, options);
		var client = new GameInterfaceMicroGaming.GameInterfaceMicroGamingClient(channel);
		var sut = new GameInterfaceMicroGamingClient(client);

		// Act
		var actual = await sut
			.PlayerBalanceAsync(
				stsApiBaseUrl: new Uri("https://sts-demo.k2net.io"),
				clientId: "01J2ZK299RA5H3N3NB5SRWN2QT",
				clientSecret: "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
				apiBaseUrl: new Uri("https://api-demo.k2net.io/api/v1"),
				agentCode: "01J2ZK4B73NTZANR5STK1WWJFZ",
				playerCode: "4ucmqplxb029",
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.True(actual.IsPlayerExist);
		Assert.Equal(10m, actual.Balance.ToDecimal());
	}
}
