namespace Tgs.GameInterface.MicroGaming.Core.UnitTest;

public sealed class DateTimeConverterTest
{
	[Fact]
	public void TestDatetimeConvertToString()
	{
		var sut = new DateTimeConverter();

		var actual = sut.StringConvertToDatetime("2022-02-16T06:34:37.113");

		Assert.Equal(new DateTimeOffset(2022, 02, 16, 06, 34, 37, 113, offset: TimeSpan.FromHours(0)), actual);
	}
}
