using System.ComponentModel;

namespace Tgs.GameInterface.MicroGaming.Core.ApplicationServices;

public record class FundStatusQuery
{
	[DefaultValue(typeof(Uri), "https://sts-demo.k2net.io")]
	public Uri StsApiBaseUrl { get; set; } = null!;

	public string ClientId { get; set; } = null!;

	public string ClientSecret { get; set; } = null!;

	[DefaultValue(typeof(Uri), "https://api-demo.k2net.io/api/v1")]
	public Uri ApiBaseUrl { get; set; } = null!;

	public string AgentCode { get; set; } = null!;

	public string ExternalOrderId { get; set; } = null!;
}
