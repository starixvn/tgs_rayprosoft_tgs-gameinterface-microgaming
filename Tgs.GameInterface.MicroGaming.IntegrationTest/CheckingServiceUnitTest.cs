using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Tgs.GameInterface.MicroGaming.Client;
using Tgs.GameInterface.MicroGaming.Core;
using Tgs.GameInterface.MicroGaming.Protos;

namespace Tgs.GameInterface.MicroGaming.IntegrationTest;

public class CheckingServiceUnitTest
{
	[Fact]
	public async Task CheckingService()
	{
		// Arrange
		var factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
			{
				services.Replace(new(typeof(IGameApiClient), Substitute.For<IGameApiClient>()));
			}));
		var options = new GrpcChannelOptions { HttpHandler = factory.Server.CreateHandler() };
		var channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, options);
		var client = new GameInterfaceMicroGaming.GameInterfaceMicroGamingClient(channel);
		var sut = new GameInterfaceMicroGamingClient(client);

		// Act
		var actual = await sut.CheckingServiceAsync(Environment.MachineName).ConfigureAwait(true);

		// Assert
		Assert.Equal(Environment.MachineName, actual);
	}
}
