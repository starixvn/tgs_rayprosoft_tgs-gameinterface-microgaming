using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tgs.GameInterface.MicroGaming.Core;

namespace Tgs.GameInterface.MicroGaming;

internal class RedisConnectionFactory : IDisposable, IRedisConnectionFactory
{
	private readonly ConnectionMultiplexer _connectionMultiplexer;
	private bool _disposedValue;

	public IConnectionMultiplexer Connection { get; }

	public IDatabase Database { get; }

	public RedisConnectionFactory(IOptions<RedisOptions> optionsAccessor)
	{
		var options = optionsAccessor.Value;

		var connectionOptions = ConfigurationOptions.Parse(options.ConnectionString);

		_connectionMultiplexer = ConnectionMultiplexer.Connect(connectionOptions);
		Connection = _connectionMultiplexer;
		Database = _connectionMultiplexer.GetDatabase();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_connectionMultiplexer?.Dispose();
			}

			_disposedValue = true;
		}
	}
}
