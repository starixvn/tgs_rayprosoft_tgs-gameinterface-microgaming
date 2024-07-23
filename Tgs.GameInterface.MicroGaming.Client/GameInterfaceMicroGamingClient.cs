using System.Runtime.CompilerServices;
using Tgs.GameInterface.MicroGaming.Protos;

namespace Tgs.GameInterface.MicroGaming.Client;

public class GameInterfaceMicroGamingClient(
	GameInterfaceMicroGaming.GameInterfaceMicroGamingClient client)
	: IGameInterfaceMicroGamingClient
{
	public async ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default)
	{
		var request = new CheckingServiceRequest
		{
			MachineName = machineName,
		};

		var reply = await client
			.CheckingServiceAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.MachineName;
	}

	public async ValueTask PlayerCreateAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerCreateRequest
		{
			StsApiBaseUrl = stsApiBaseUrl.ToString(),
			ClientId = clientId,
			ClientSecret = clientSecret,
			ApiBaseUrl = apiBaseUrl.ToString(),
			AgentCode = agentCode,

			PlayerCode = playerCode,
		};

		_ = await client
			.PlayerCreateAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerBalanceRequest
		{
			StsApiBaseUrl = stsApiBaseUrl.ToString(),
			ClientId = clientId,
			ClientSecret = clientSecret,
			ApiBaseUrl = apiBaseUrl.ToString(),
			AgentCode = agentCode,

			PlayerCode = playerCode,
		};

		return await client
			.PlayerBalanceAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<GameUrlReply> GameUrlAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerCode, string language, GamePlatform platform, string gameCode, CancellationToken cancellationToken = default)
	{
		var request = new GameUrlRequest
		{
			StsApiBaseUrl = stsApiBaseUrl.ToString(),
			ClientId = clientId,
			ClientSecret = clientSecret,
			ApiBaseUrl = apiBaseUrl.ToString(),
			AgentCode = agentCode,

			PlayerCode = playerCode,
			Language = language,
			Platform = platform,
			GameCode = gameCode,
		};

		return await client
			.GameUrlAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async IAsyncEnumerable<GameRecordInfo> GameRecordAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, int limit, string startingAfter, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new GameRecordRequest
		{
			StsApiBaseUrl = stsApiBaseUrl.ToString(),
			ClientId = clientId,
			ClientSecret = clientSecret,
			ApiBaseUrl = apiBaseUrl.ToString(),
			AgentCode = agentCode,

			Limit = limit,
			StarttingAfter = startingAfter,
		};

		var reply = client.GameRecord(
			request: request,
			cancellationToken: cancellationToken);

		while (await reply.ResponseStream.MoveNext(cancellationToken).ConfigureAwait(false))
			yield return reply.ResponseStream.Current;
	}

	public async ValueTask<FundReply> FundInAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerCode, decimal amount, string orderId, CancellationToken cancellationToken = default)
	{
		var request = new FundRequest
		{
			StsApiBaseUrl = stsApiBaseUrl.ToString(),
			ClientId = clientId,
			ClientSecret = clientSecret,
			ApiBaseUrl = apiBaseUrl.ToString(),
			AgentCode = agentCode,

			PlayerCode = playerCode,
			Amount = amount,
			OrderId = orderId,
		};

		return await client
			.FundInAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<FundReply> FundOutAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerCode, decimal amount, string orderId, CancellationToken cancellationToken = default)
	{
		var request = new FundRequest
		{
			StsApiBaseUrl = stsApiBaseUrl.ToString(),
			ClientId = clientId,
			ClientSecret = clientSecret,
			ApiBaseUrl = apiBaseUrl.ToString(),
			AgentCode = agentCode,

			PlayerCode = playerCode,
			Amount = amount,
			OrderId = orderId,
		};

		return await client
			.FundOutAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<BaseReply> FundStatusAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string externalOrderId, CancellationToken cancellationToken = default)
	{
		var request = new FundStatusRequest
		{
			StsApiBaseUrl = stsApiBaseUrl.ToString(),
			ClientId = clientId,
			ClientSecret = clientSecret,
			ApiBaseUrl = apiBaseUrl.ToString(),
			AgentCode = agentCode,

			ExternalOrderId = externalOrderId,
		};

		return await client
			.FundStatusAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
