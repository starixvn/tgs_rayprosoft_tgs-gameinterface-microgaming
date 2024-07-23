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

public class GameRecordUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var serviceRequest = new GameRecordBetLogQuery
		{
			StsApiBaseUrl = new Uri("https://sts-demo.k2net.io"),
			ClientId = "01J2ZK299RA5H3N3NB5SRWN2QT",
			ClientSecret = "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
			ApiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1"),
			AgentCode = "01J2ZK4B73NTZANR5STK1WWJFZ",
			Limit = 20000,
			StartingAfter = "",
		};
		var result = new GameRecordResult(
			PlayerCode: "4ucmqplxb029",
			ExternalBetId: "01J31SE9W7843T5R21DZ3GVNBK",
			ExternalGameId: "01J31SEJ71J7AFA639M6C8PMHQ",
			ExternalCreatedOn: new DateTimeOffset(2024, 07, 18, 10, 12, 00, TimeSpan.Zero),
			ExternalClosedOn: new DateTimeOffset(2024, 07, 18, 10, 15, 00, TimeSpan.Zero),
			BetStatus: Core.BetStatus.Closed,
			Bet: 16m,
			Settle: 32m,
			WinLoss: -16m,
			Jackpot: 0m,
			AdditionalData: "01J31SHKFTHECRD65AVK89ZQ45");
		var fakerService = Substitute.For<IQueryService<GameRecordBetLogQuery, GameRecordResult>>();
		_ = fakerService
			.QueryAsync(
				query: Arg.Is(serviceRequest),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new[] { result }.ToAsyncEnumerable());
		var factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
			{
				services.Replace(new(typeof(IGameApiClient), Substitute.For<IGameApiClient>()));
				services.Replace(new(typeof(IQueryService<GameRecordBetLogQuery, GameRecordResult>), fakerService));
			}));
		var options = new GrpcChannelOptions { HttpHandler = factory.Server.CreateHandler() };
		var channel = GrpcChannel.ForAddress(factory.Server.BaseAddress, options);
		var client = new GameInterfaceMicroGaming.GameInterfaceMicroGamingClient(channel);
		var sut = new GameInterfaceMicroGamingClient(client);

		// Act
		var results = sut
			.GameRecordAsync(
				stsApiBaseUrl: new Uri("https://sts-demo.k2net.io"),
				clientId: "01J2ZK299RA5H3N3NB5SRWN2QT",
				clientSecret: "8456c9de-d589-4e8a-876f-c9c8e48dda3e",
				apiBaseUrl: new Uri("https://api-demo.k2net.io/api/v1"),
				agentCode: "01J2ZK4B73NTZANR5STK1WWJFZ",
				limit: 20000,
				startingAfter: "",
				cancellationToken: default);

		// Assert
		await foreach (var actual in results)
		{
			Assert.NotNull(actual);
			Assert.Equal("4ucmqplxb029", actual.PlayerCode);
			Assert.Equal(16m, actual.Bet.ToDecimal());
			Assert.Equal(32m, actual.Settle.ToDecimal());
			Assert.Equal(-16m, actual.WinLoss.ToDecimal());
			Assert.Equal(0m, actual.Jackpot.ToDecimal());
			Assert.Equal(new DateTimeOffset(2024, 07, 18, 10, 12, 00, TimeSpan.Zero), actual.ExternalCreatedOn.ToDateTimeOffset());
			Assert.Equal(new DateTimeOffset(2024, 07, 18, 10, 15, 00, TimeSpan.Zero), actual.ExternalClosedOn.ToDateTimeOffset());
			Assert.Equal("01J31SE9W7843T5R21DZ3GVNBK", actual.ExternalBetId);
			Assert.Equal("01J31SEJ71J7AFA639M6C8PMHQ", actual.ExternalGameId);
			Assert.Equal("01J31SHKFTHECRD65AVK89ZQ45", actual.AdditionalData);
		}
	}
}
