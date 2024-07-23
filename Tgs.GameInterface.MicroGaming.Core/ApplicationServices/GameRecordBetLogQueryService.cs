using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

internal class GameRecordBetLogQueryService(IGameApiClient apiClient) : IQueryService<GameRecordBetLogQuery, GameRecordResult>
{
	public async IAsyncEnumerable<GameRecordResult> QueryAsync(GameRecordBetLogQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var responseItems = apiClient.BetsAsync(
			stsApiBaseUrl: query.StsApiBaseUrl,
			clientId: query.ClientId,
			clientSecret: query.ClientSecret,
			apiBaseUrl: query.ApiBaseUrl,
			agentCode: query.AgentCode,
			limit: query.Limit,
			startingAfter: query.StartingAfter,
			cancellationToken: cancellationToken);

		await foreach (var responseItem in responseItems)
		{
			var bet = responseItem.BetAmount;
			var jackpot = responseItem.PayoutAmount;
			yield return new GameRecordResult(
				PlayerCode: responseItem.PlayerId,
				ExternalBetId: responseItem.BetUid,
				ExternalGameId: responseItem.GameCode,
				ExternalCreatedOn: responseItem.CreatedDateUtc,
				ExternalClosedOn: responseItem.GameEndTimeUtc,
				BetStatus: responseItem.BetStatus,
				Bet: bet,
				Settle: jackpot + bet + bet,
				WinLoss: jackpot - bet,
				Jackpot: jackpot,
				AdditionalData: responseItem.AdditionalData);
		}
	}
}
