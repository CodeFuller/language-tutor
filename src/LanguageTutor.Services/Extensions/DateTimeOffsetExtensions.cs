using System;

namespace LanguageTutor.Services.Extensions
{
	internal static class DateTimeOffsetExtensions
	{
		public static DateOnly ToDateOnly(this DateTimeOffset dateTime)
		{
			return DateOnly.FromDateTime(dateTime.Date);
		}
	}
}
