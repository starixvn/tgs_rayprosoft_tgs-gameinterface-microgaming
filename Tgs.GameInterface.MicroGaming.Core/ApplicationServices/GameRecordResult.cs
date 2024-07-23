namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

public record class GameRecordResult(
	string PlayerCode,
	string ExternalBetId,
	string ExternalGameId,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	BetStatus BetStatus,
	decimal Bet,
	decimal Jackpot,
	string AdditionalData);
