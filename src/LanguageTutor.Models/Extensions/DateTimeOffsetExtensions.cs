using System;

namespace LanguageTutor.Models.Extensions
{
	public static class DateTimeOffsetExtensions
	{
		public static DateOnly ToDateOnly(this DateTimeOffset dateTime)
		{
			return DateOnly.FromDateTime(dateTime.Date);
		}
	}
}
