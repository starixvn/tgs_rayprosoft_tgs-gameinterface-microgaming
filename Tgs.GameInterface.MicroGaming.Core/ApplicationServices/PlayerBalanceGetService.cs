using Tgs.ServiceUtility;

namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

internal class PlayerBalanceGetService(IGameApiClient apiClient) : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var (isPlayerExist, balance) = await apiClient
			.PlayersDetailsAsync(
				stsApiBaseUrl: query.StsApiBaseUrl,
				clientId: query.ClientId,
				clientSecret: query.ClientSecret,
				apiBaseUrl: query.ApiBaseUrl,
				agentCode: query.AgentCode,
				playerId: query.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerBalanceResult(isPlayerExist, balance);
	}
}
