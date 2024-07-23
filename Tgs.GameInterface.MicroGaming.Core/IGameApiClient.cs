using Tgs.GameInterface.MicroGaming.Core.Models;

namespace Tgs.GameInterface.MicroGaming.Core;

public interface IGameApiClient
{
	ValueTask PlayersAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerId, CancellationToken cancellationToken = default);

	ValueTask<(bool isPlayerExist, decimal balance)> PlayersDetailsAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerId, CancellationToken cancellationToken = default);

	ValueTask<string> PlayersSessionsAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerId, string langCode, GamePlatform platform, string contentCode, CancellationToken cancellationToken = default);

	IAsyncEnumerable<BetsResponseItem> BetsAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, int limit, string startingAfter, CancellationToken cancellationToken = default);

	ValueTask<WalletTransactionsResponse> WalletTransactionsAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerId, string type, decimal amount, string externalTransactionId, CancellationToken cancellationToken = default);

	ValueTask<bool> WalletTransactionsDetailsAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string walletTransactionId, CancellationToken cancellationToken = default);
}
