using Tgs.ServiceUtility;

namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

internal class FundOutGetService(IGameApiClient apiClient) : IGetService<FundOutQuery, FundResult>
{
	public async ValueTask<FundResult> GetAsync(FundOutQuery query, CancellationToken cancellationToken = default)
	{
		var response = await apiClient
			.WalletTransactionsAsync(
				stsApiBaseUrl: query.StsApiBaseUrl,
				clientId: query.ClientId,
				clientSecret: query.ClientSecret,
				apiBaseUrl: query.ApiBaseUrl,
				agentCode: query.AgentCode,
				playerId: query.PlayerCode,
				type: "Deposit",
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
