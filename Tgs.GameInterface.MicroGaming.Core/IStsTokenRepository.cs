namespace Tgs.GameInterface.MicroGaming.Core;

public interface IStsTokenRepository
{
	ValueTask<string> GetTokenAsync(Uri apiBaseUrl, string clientId, string clientSecret, CancellationToken cancellationToken = default);
}
