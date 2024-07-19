namespace Tgs.GameInterface.MicroGaming.Core.Models;

public record class WalletTransactionsResponse(
	string Id,
	string ExternalTransactionId,
	decimal Amount,
	string Resp);
