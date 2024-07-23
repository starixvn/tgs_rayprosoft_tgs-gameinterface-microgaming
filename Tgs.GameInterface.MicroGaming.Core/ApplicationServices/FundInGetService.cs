using Tgs.ServiceUtility;

namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

internal class FundInGetService(IGameApiClient apiClient) : IGetService<FundInQuery, FundResult>
{
	public async ValueTask<FundResult> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
	{
		var response = await apiClient
			.WalletTransactionsAsync(
				stsApiBaseUrl: query.StsApiBaseUrl,
				clientId: query.ClientId,
				clientSecret: query.ClientSecret,
				apiBaseUrl: query.ApiBaseUrl,
				agentCode: query.AgentCode,
				playerId: query.PlayerCode,
				type: "Withdraw",
				amount: query.Amount,
				externalTransactionId: query.OrderId,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new FundResult(
			OrderId: response.ExternalTransactionId,
			ExternalOrderId: response.Id,
			Resp: response.Resp);
	}
}
