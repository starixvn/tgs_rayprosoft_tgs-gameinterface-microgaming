namespace Tgs.GameInterface.MicroGaming.Core;

public interface IDateTimeConverter
{
	DateTimeOffset StringConvertToDatetime(string str);
}
