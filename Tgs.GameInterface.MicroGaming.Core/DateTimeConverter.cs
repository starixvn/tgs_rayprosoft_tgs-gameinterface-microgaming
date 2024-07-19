namespace Tgs.GameInterface.MicroGaming.Core;

internal class DateTimeConverter : IDateTimeConverter
{
	private readonly TimeZoneInfo _timeZone = TimeZoneInfo.Utc;

	public DateTimeOffset StringConvertToDatetime(string str)
		=> DateTime.TryParse(str, out var date) ? new(date, _timeZone.GetUtcOffset(date)) : DateTimeOffset.UnixEpoch;
}
