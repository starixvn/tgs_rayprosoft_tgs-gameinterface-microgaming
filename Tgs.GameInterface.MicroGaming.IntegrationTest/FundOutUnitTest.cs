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

public class FundOutUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var serviceRequest = new FundOutQuery
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			PlayerCode = "4ucmqplxb029",
			Amount = 16m,
			OrderId = "01J31RYE9SPCCP1NF9YZC5FETX",
		};
		var fakerService = Substitute.For<IGetService<FundOutQuery, FundResult>>();
		_ = fakerService
			.GetAsync(
				query: Arg.Is(serviceRequest),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(new FundResult(
				OrderId: "01J31RYE9SPCCP1NF9YZC5FETX",
				ExternalOrderId: "01J31RYNWRK8W4VF4TC4MDHRJ7",
				Resp: "01J31RZ4GR32FM8QZCH34CDDQT")));
		var factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
			{
				services.Replace(new(typeof(IGameApiClient), Substitute.For<IGameApiClient>()));
				services.Replace(new(typeof(IGetService<FundOutQuery, FundResult>), fakerService));
			}));
		var options = new GrpcChannelOptions { HttpHandler = factory.Server.CreateHandler() };
		var channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, options);
		var client = new GameInterfaceMicroGaming.GameInterfaceMicroGamingClient(channel);
		var sut = new GameInterfaceMicroGamingClient(client);

		// Act
		var actual = await sut
			.FundOutAsync(
				stsApiBaseUrl: new Uri("https://sts-demo.k2net.io"),
				clientId: "01J2ZK299RA5H3N3NB5SRWN2QT",
				clientSecret: "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
				apiBaseUrl: new Uri("https://api-demo.k2net.io/api/v1"),
				agentCode: "01J2ZK4B73NTZANR5STK1WWJFZ",
				playerCode: "4ucmqplxb029",
				amount: 16m,
				orderId: "01J31RYE9SPCCP1NF9YZC5FETX",
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.Equal("01J31RYE9SPCCP1NF9YZC5FETX", actual.OrderId);
		Assert.Equal("01J31RYNWRK8W4VF4TC4MDHRJ7", actual.ExternalOrderId);
		Assert.Equal("01J31RZ4GR32FM8QZCH34CDDQT", actual.Resp);
	}
}
