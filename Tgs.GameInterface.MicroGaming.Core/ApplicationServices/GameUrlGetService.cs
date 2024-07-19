using Tgs.ServiceUtility;

namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

internal class GameUrlGetService(IGameApiClient apiClient) : IGetService<GameUrlQuery, string>
{
	public ValueTask<string> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
		=> apiClient.PlayersSessionsAsync(
			stsApiBaseUrl: query.StsApiBaseUrl,
			clientId: query.ClientId,
			clientSecret: query.ClientSecret,
			apiBaseUrl: query.ApiBaseUrl,
			agentCode: query.AgentCode,
			playerId: query.PlayerCode,
			langCode: query.Language,
			platform: query.Platform,
			contentCode: query.GameCode,
			cancellationToken: cancellationToken);
}
