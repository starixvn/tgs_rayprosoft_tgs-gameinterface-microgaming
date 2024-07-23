using Tgs.ServiceUtility;

namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

internal class PlayerCreateCommandService(IGameApiClient apiClient) : ICommandService<PlayerCreateCommand>
{
	public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
		=> apiClient.PlayersAsync(
			stsApiBaseUrl: command.StsApiBaseUrl,
			clientId: command.ClientId,
			clientSecret: command.ClientSecret,
			apiBaseUrl: command.ApiBaseUrl,
			agentCode: command.AgentCode,
			playerId: command.PlayerCode,
			cancellationToken: cancellationToken);
}
