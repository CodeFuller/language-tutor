using System;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.UnitTests.Helpers
{
	internal static class DateTimeExtensions
	{
		public static CheckResult ToSuccessfulCheckResult(this DateTime dateTime)
		{
			return new CheckResult
			{
				DateTime = dateTime,
				CheckResultType = CheckResultType.Ok,
			};
		}

		public static CheckResult ToFailedCheckResult(this DateTime dateTime)
		{
			return new CheckResult
			{
				DateTime = dateTime,
				CheckResultType = CheckResultType.Misspelled,
			};
		}
	}
}
