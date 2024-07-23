using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.MicroGaming.Core.ApplicationServices;
using Tgs.GameInterface.MicroGaming.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.MicroGaming.GrpcServices;

public class GameInterfaceMicroGamingGrpcService(
	ILogger<GameInterfaceMicroGamingGrpcService> logger,
	ICommandService<PlayerCreateCommand> playerCreateService,
	IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
	IGetService<GameUrlQuery, string> gameUrlService,
	IQueryService<GameRecordBetLogQuery, GameRecordResult> gameRecordService,
	IGetService<FundInQuery, FundResult> fundInService,
	IGetService<FundOutQuery, FundResult> fundOutService,
	IGetService<FundStatusQuery, bool> fundStatusService)
	: GameInterfaceMicroGaming.GameInterfaceMicroGamingBase
{
	public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
	{
		logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

		return Task.FromResult(new CheckingServiceReply
		{
			MachineName = Environment.MachineName
		});
	}

	public override async Task<BaseReply> PlayerCreate(PlayerCreateRequest request, ServerCallContext context)
	{
		var command = new PlayerCreateCommand
		{
			StsApiBaseUrl = new Uri(request.StsApiBaseUrl),
			ClientId = request.ClientId,
			ClientSecret = request.ClientSecret,
			ApiBaseUrl = new Uri(request.ApiBaseUrl),
			AgentCode = request.AgentCode,

			PlayerCode = request.PlayerCode,
		};

		await playerCreateService
			.ExecuteAsync(
				command: command,
				cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new BaseReply
		{
			Status = true
		};
	}

	public override async Task<PlayerBalanceReply> PlayerBalance(PlayerBalanceRequest request, ServerCallContext context)
	{
		var query = new PlayerBalanceQuery
		{
			StsApiBaseUrl = new Uri(request.StsApiBaseUrl),
			ClientId = request.ClientId,
			ClientSecret = request.ClientSecret,
			ApiBaseUrl = new Uri(request.ApiBaseUrl),
			AgentCode = request.AgentCode,

			PlayerCode = request.PlayerCode,
		};

		var result = await playerBalanceService
			.GetAsync(
				query: query,
				cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new PlayerBalanceReply
		{
			IsPlayerExist = result.IsPlayerExist,
			Balance = result.Balance,
		};
	}

	public override async Task<GameUrlReply> GameUrl(GameUrlRequest request, ServerCallContext context)
	{
		var query = new GameUrlQuery
		{
			StsApiBaseUrl = new Uri(request.StsApiBaseUrl),
			ClientId = request.ClientId,
			ClientSecret = request.ClientSecret,
			ApiBaseUrl = new Uri(request.ApiBaseUrl),
			AgentCode = request.AgentCode,

			PlayerCode = request.PlayerCode,
			Language = request.Language,
			Platform = request.Platform switch
			{
				GamePlatform.Desktop => Core.GamePlatform.Desktop,
				GamePlatform.Mobile => Core.GamePlatform.Mobile,
				_ => Core.GamePlatform.Unknown
			},
			GameCode = request.GameCode,
		};

		var result = await gameUrlService
			.GetAsync(
				query: query,
				cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new GameUrlReply
		{
			GameUrl = result,
		};
	}

	public override async Task GameRecord(GameRecordRequest request, IServerStreamWriter<GameRecordInfo> responseStream, ServerCallContext context)
	{
		var query = new GameRecordBetLogQuery
		{
			StsApiBaseUrl = new Uri(request.StsApiBaseUrl),
			ClientId = request.ClientId,
			ClientSecret = request.ClientSecret,
			ApiBaseUrl = new Uri(request.ApiBaseUrl),
			AgentCode = request.AgentCode,

			Limit = request.Limit,
			StartingAfter = request.StarttingAfter,
		};

		var results = gameRecordService.QueryAsync(
			query: query,
			cancellationToken: context.CancellationToken);

		await foreach (var record in results)
		{
			await responseStream
				.WriteAsync(new GameRecordInfo
				{
					PlayerCode = record.PlayerCode,
					ExternalBetId = record.ExternalBetId,
					ExternalGameId = record.ExternalGameId,
					ExternalCreatedOn = record.ExternalCreatedOn.ToTimestamp(),
					ExternalClosedOn = record.ExternalClosedOn.ToTimestamp(),
					BetStatus = record.BetStatus switch
					{
						Core.BetStatus.Cancelled => BetStatus.Cancelled,
						_ => BetStatus.Closed,
					},
					Bet = record.Bet,
					Settle = record.Settle,
					WinLoss = record.WinLoss,
					AdditionalData = record.AdditionalData
				})
				.ConfigureAwait(false);
		}
	}

	public override async Task<FundReply> FundIn(FundRequest request, ServerCallContext context)
	{
		var query = new FundInQuery
		{
			StsApiBaseUrl = new Uri(request.StsApiBaseUrl),
			ClientId = request.ClientId,
			ClientSecret = request.ClientSecret,
			ApiBaseUrl = new Uri(request.ApiBaseUrl),
			AgentCode = request.AgentCode,

			PlayerCode = request.PlayerCode,
			Amount = request.Amount,
			OrderId = request.OrderId,
		};

		var result = await fundInService
			.GetAsync(
				query: query,
				cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new FundReply
		{
			OrderId = result.OrderId,
			ExternalOrderId = result.ExternalOrderId,
			Resp = result.Resp,
		};
	}

	public override async Task<FundReply> FundOut(FundRequest request, ServerCallContext context)
	{
		var query = new FundOutQuery
		{
			StsApiBaseUrl = new Uri(request.StsApiBaseUrl),
			ClientId = request.ClientId,
			ClientSecret = request.ClientSecret,
			ApiBaseUrl = new Uri(request.ApiBaseUrl),
			AgentCode = request.AgentCode,

			PlayerCode = request.PlayerCode,
			Amount = request.Amount,
			OrderId = request.OrderId,
		};

		var result = await fundOutService
			.GetAsync(
				query: query,
				cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new FundReply
		{
			OrderId = result.OrderId,
			ExternalOrderId = result.ExternalOrderId,
			Resp = result.Resp,
		};
	}

	public override async Task<BaseReply> FundStatus(FundStatusRequest request, ServerCallContext context)
	{
		var query = new FundStatusQuery
		{
			StsApiBaseUrl = new Uri(request.StsApiBaseUrl),
			ClientId = request.ClientId,
			ClientSecret = request.ClientSecret,
			ApiBaseUrl = new Uri(request.ApiBaseUrl),
			AgentCode = request.AgentCode,

			ExternalOrderId = request.ExternalOrderId,
		};

		var result = await fundStatusService
			.GetAsync(
				query: query,
				cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new BaseReply
		{
			Status = result,
		};
	}
}
