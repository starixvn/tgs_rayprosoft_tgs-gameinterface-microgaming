namespace Tgs.GameInterface.MicroGaming.Core.Models;

public record class BetsResponseItem(
	string BetUid,
	DateTimeOffset CreatedDateUtc,
	DateTimeOffset GameStartTimeUtc,
	DateTimeOffset GameEndTimeUtc,
	string GameCode,
	string PlayerId,
	string Currency,
	decimal BetAmount,
	decimal PayoutAmount,
	BetStatus BetStatus,
	string ExternalTransactionId,
	string AdditionalData);
