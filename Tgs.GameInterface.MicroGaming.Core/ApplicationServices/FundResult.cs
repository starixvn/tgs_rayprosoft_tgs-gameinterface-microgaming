namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

public record class FundResult(
	string OrderId,
	string ExternalOrderId,
	string Resp);
