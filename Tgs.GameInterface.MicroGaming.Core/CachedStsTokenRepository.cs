using StackExchange.Redis;

namespace Tgs.GameInterface.MicroGaming.Core;

internal class CachedStsTokenRepository(IDatabase database, IStsTokenRepository baseRepository) : IStsTokenRepository
{
	public async ValueTask<string> GetTokenAsync(Uri apiBaseUrl, string clientId, string clientSecret, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(CachedStsTokenRepository).Namespace!.Replace(".", "-").ToLower(), ":", clientId);

		var cachedToken = await database.StringGetAsync(key).ConfigureAwait(false);

		if (cachedToken.HasValue)
			return cachedToken!;

		var newlyToken = await baseRepository.GetTokenAsync(apiBaseUrl, clientId, clientSecret, cancellationToken).ConfigureAwait(false);

		await database.StringSetAsync(key, newlyToken, expiry: TimeSpan.FromSeconds(1800)).ConfigureAwait(false);

		return newlyToken;
	}
}
