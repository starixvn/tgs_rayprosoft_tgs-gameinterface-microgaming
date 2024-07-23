namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

public record class PlayerBalanceResult(
	bool IsPlayerExist,
	decimal Balance);
