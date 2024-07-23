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

public class FundInUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var serviceRequest = new FundInQuery
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			PlayerCode = "4ucmqplxb029",
			Amount = 16m,
			OrderId = "01J31R7ARQB5N1AKJC3JDNBN3D",
		};
		var fakerService = Substitute.For<IGetService<FundInQuery, FundResult>>();
		_ = fakerService
			.GetAsync(
				query: Arg.Is(serviceRequest),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(new FundResult(
				OrderId: "01J31R7ARQB5N1AKJC3JDNBN3D",
				ExternalOrderId: "01J31R9GKP9XY0R6EC6DD2STA4",
				Resp: "01J31R9X6DHRNPFNRPTA4JSXM5")));
		var factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
			{
				services.Replace(new(typeof(IGameApiClient), Substitute.For<IGameApiClient>()));
				services.Replace(new(typeof(IGetService<FundInQuery, FundResult>), fakerService));
			}));
		var options = new GrpcChannelOptions { HttpHandler = factory.Server.CreateHandler() };
		var channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, options);
		var client = new GameInterfaceMicroGaming.GameInterfaceMicroGamingClient(channel);
		var sut = new GameInterfaceMicroGamingClient(client);

		// Act
		var actual = await sut
			.FundInAsync(
				stsApiBaseUrl: new Uri("https://sts-demo.k2net.io"),
				clientId: "01J2ZK299RA5H3N3NB5SRWN2QT",
				clientSecret: "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
				apiBaseUrl: new Uri("https://api-demo.k2net.io/api/v1"),
				agentCode: "01J2ZK4B73NTZANR5STK1WWJFZ",
				playerCode: "4ucmqplxb029",
				amount: 16m,
				orderId: "01J31R7ARQB5N1AKJC3JDNBN3D",
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.Equal("01J31R7ARQB5N1AKJC3JDNBN3D", actual.OrderId);
		Assert.Equal("01J31R9GKP9XY0R6EC6DD2STA4", actual.ExternalOrderId);
		Assert.Equal("01J31R9X6DHRNPFNRPTA4JSXM5", actual.Resp);
	}
}
