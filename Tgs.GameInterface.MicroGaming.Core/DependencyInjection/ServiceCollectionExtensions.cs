using System.Reflection;
using Tgs.GameInterface.MicroGaming.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameInterfaceCore(
		this IServiceCollection services)
		=> services
		.AddHttpClient<StsTokenRepository>()
		.Services
		.AddTransient<IStsTokenRepository, StsTokenRepository>()
		.Decorate<IStsTokenRepository>((baseService, provider) =>
		{
			var factor = provider.GetRequiredService<IRedisConnectionFactory>();
			var database = factor.Database;
			return new CachedStsTokenRepository(database, baseService);
		})
		.RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
		.AddSingleton<IDateTimeConverter, DateTimeConverter>()
		.AddHttpClient<IGameApiClient, GameApiClient>()
		.Services;
}
