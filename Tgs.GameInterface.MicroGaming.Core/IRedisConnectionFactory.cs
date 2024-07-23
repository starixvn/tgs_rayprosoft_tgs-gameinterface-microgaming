using StackExchange.Redis;

namespace Tgs.GameInterface.MicroGaming.Core;

public interface IRedisConnectionFactory
{
	IDatabase Database { get; }
}
