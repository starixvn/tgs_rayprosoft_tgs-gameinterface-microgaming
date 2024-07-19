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

public class GameUrlUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var serviceRequest = new GameUrlQuery
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			PlayerCode = "4ucmqplxb029",
			Language = "zh-CN",
			Platform = Core.GamePlatform.Mobile,
			GameCode = "01J31R0THWJ20V61D8GVAV5BAZ",
		};
		var fakerService = Substitute.For<IGetService<GameUrlQuery, string>>();
		_ = fakerService
			.GetAsync(
				query: Arg.Is(serviceRequest),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult("https://xxx.xxx.xxx/xxx"));
		var factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
			{
				services.Replace(new(typeof(IGameApiClient), Substitute.For<IGameApiClient>()));
				services.Replace(new(typeof(IGetService<GameUrlQuery, string>), fakerService));
			}));
		var options = new GrpcChannelOptions { HttpHandler = factory.Server.CreateHandler() };
		var channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, options);
		var client = new GameInterfaceMicroGaming.GameInterfaceMicroGamingClient(channel);
		var sut = new GameInterfaceMicroGamingClient(client);

		// Act
		var actual = await sut
			.GameUrlAsync(
				stsApiBaseUrl: new Uri("https://sts-demo.k2net.io"),
				clientId: "01J2ZK299RA5H3N3NB5SRWN2QT",
				clientSecret: "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
				apiBaseUrl: new Uri("https://api-demo.k2net.io/api/v1"),
				agentCode: "01J2ZK4B73NTZANR5STK1WWJFZ",
				playerCode: "4ucmqplxb029",
				language: "zh-CN",
				platform: Protos.GamePlatform.Mobile,
				gameCode: "01J31R0THWJ20V61D8GVAV5BAZ",
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.Equal("https://xxx.xxx.xxx/xxx", actual.GameUrl);
	}
}
