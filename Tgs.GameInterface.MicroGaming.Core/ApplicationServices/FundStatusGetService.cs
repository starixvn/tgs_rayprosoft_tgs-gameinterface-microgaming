using Tgs.ServiceUtility;

namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

internal class FundStatusGetService(IGameApiClient apiClient) : IGetService<FundStatusQuery, bool>
{
	public ValueTask<bool> GetAsync(FundStatusQuery query, CancellationToken cancellationToken = default)
		=> apiClient.WalletTransactionsDetailsAsync(
			stsApiBaseUrl: query.StsApiBaseUrl,
			clientId: query.ClientId,
			clientSecret: query.ClientSecret,
			apiBaseUrl: query.ApiBaseUrl,
			agentCode: query.AgentCode,
			walletTransactionId: query.ExternalOrderId,
			cancellationToken: cancellationToken);
}
