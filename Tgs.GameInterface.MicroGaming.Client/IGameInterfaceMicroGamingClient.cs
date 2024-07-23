using Tgs.GameInterface.MicroGaming.Protos;

namespace Tgs.GameInterface.MicroGaming.Client;

public interface IGameInterfaceMicroGamingClient
{
	ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default);

	ValueTask PlayerCreateAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<GameUrlReply> GameUrlAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerCode, string language, GamePlatform platform, string gameCode, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameRecordInfo> GameRecordAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, int limit, string startingAfter, CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundInAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerCode, decimal amount, string orderId, CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundOutAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerCode, decimal amount, string orderId, CancellationToken cancellationToken = default);

	ValueTask<BaseReply> FundStatusAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string externalOrderId, CancellationToken cancellationToken = default);
}
