using System;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Data
{
	public class CheckResults
	{
		public int CheckedTextsCount { get; private set; }

		public int CorrectTextsCount { get; private set; }

		public int IncorrectTextsCount { get; private set; }

		public int SkippedTextsCount { get; private set; }

		public void AddResult(CheckResultType checkResultType)
		{
			++CheckedTextsCount;

			switch (checkResultType)
			{
				case CheckResultType.Ok:
					++CorrectTextsCount;
					break;

				case CheckResultType.Misspelled:
					++IncorrectTextsCount;
					break;

				case CheckResultType.Skipped:
					++SkippedTextsCount;
					break;

				default:
					throw new NotSupportedException($"Check result type is not supported: {checkResultType}");
			}
		}
	}
}
