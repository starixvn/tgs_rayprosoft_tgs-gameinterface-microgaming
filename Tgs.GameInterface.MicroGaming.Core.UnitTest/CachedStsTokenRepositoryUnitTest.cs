using NSubstitute;
using StackExchange.Redis;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest;

public class CachedStsTokenRepositoryUnitTest
{
	[Fact]
	public async Task HitFromCache()
	{
		// Arrange
		var apiBaseUrl = new Uri("https://sts-demo.k2net.io");
		var clientId = "abc123";
		var clientSecret = "01J2WPK2XEN8CK2MR2PB5N39JX";

		var key = string.Concat(typeof(CachedStsTokenRepository).Namespace!.Replace(".", "-").ToLower(), ":", clientId);
		var fakerDatabase = Substitute.For<IDatabase>();
		fakerDatabase
			.StringGetAsync(key)
			.Returns("01J2WQGNSWTZMPTS82P60AP505");

		var fakerBaseRepository = Substitute.For<IStsTokenRepository>();

		var sut = new CachedStsTokenRepository(fakerDatabase, fakerBaseRepository);

		// Act
		var actual = await sut
			.GetTokenAsync(
				apiBaseUrl: apiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.Equal("01J2WQGNSWTZMPTS82P60AP505", actual);
	}

	[Fact]
	public async Task RenewFromBaseRepoThenCache()
	{
		// Arrange
		var apiBaseUrl = new Uri("https://sts-demo.k2net.io");
		var clientId = "abc123";
		var clientSecret = "01J2WPK2XEN8CK2MR2PB5N39JX";

		var key = string.Concat(typeof(CachedStsTokenRepository).Namespace!.Replace(".", "-").ToLower(), ":", clientId);
		var fakerDatabase = Substitute.For<IDatabase>();
		fakerDatabase
			.StringGetAsync(key)
			.Returns(RedisValue.Null);

		var fakerBaseRepository = Substitute.For<IStsTokenRepository>();
		fakerBaseRepository
			.GetTokenAsync(
				apiBaseUrl: apiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				cancellationToken: default)
			.Returns("01J2WQKP0R7XV9YPNZ8AHKJ79Y");

		var sut = new CachedStsTokenRepository(fakerDatabase, fakerBaseRepository);

		// Act
		var actual = await sut
			.GetTokenAsync(
				apiBaseUrl: apiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.Equal("01J2WQKP0R7XV9YPNZ8AHKJ79Y", actual);

		_ = fakerDatabase
			.Received(1)
			.StringSetAsync(key, "01J2WQKP0R7XV9YPNZ8AHKJ79Y", expiry: TimeSpan.FromSeconds(1800))
			.ConfigureAwait(true);
	}
}
