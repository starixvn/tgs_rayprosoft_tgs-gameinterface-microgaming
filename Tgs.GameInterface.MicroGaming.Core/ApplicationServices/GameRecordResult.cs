namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

public record class GameRecordResult(
	string PlayerCode,
	string ExternalBetId,
	string ExternalGameId,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	BetStatus BetStatus,
	decimal Bet,
	decimal Settle,
	decimal WinLoss,
	decimal Jackpot,
	string AdditionalData);
