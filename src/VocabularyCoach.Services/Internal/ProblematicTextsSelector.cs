using System;
using System.Collections.Generic;
using System.Linq;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Extensions;

namespace VocabularyCoach.Services.Internal
{
	internal class ProblematicTextsSelector : IProblematicTextsSelector
	{
		private class CheckResultTypeComparer : IComparer<CheckResultType>
		{
			public int Compare(CheckResultType x, CheckResultType y)
			{
				return GetResultTypeValue(x).CompareTo(GetResultTypeValue(y));
			}

			private static int GetResultTypeValue(CheckResultType checkResultType)
			{
				return checkResultType switch
				{
					CheckResultType.Ok => 0,
					CheckResultType.Misspelled => -1,
					CheckResultType.Skipped => -2,
					_ => throw new NotSupportedException($"Check result type is not supported: {checkResultType}"),
				};
			}
		}

		private class ProblematicTextsCompare : IComparer<StudiedText>
		{
			private readonly CheckResultTypeComparer checkResultTypeComparer = new();

			public int Compare(StudiedText x, StudiedText y)
			{
				foreach (var (first, second) in x.CheckResults.Zip(y.CheckResults))
				{
					var checkResultTypeCmp = checkResultTypeComparer.Compare(first.CheckResultType, second.CheckResultType);
					if (checkResultTypeCmp != 0)
					{
						return checkResultTypeCmp;
					}
				}

				var failedCount1 = x.CheckResults.Count(c => c.IsFailed);
				var failedCount2 = y.CheckResults.Count(c => c.IsFailed);
				var failedCountCmp = failedCount1.CompareTo(failedCount2);
				if (failedCountCmp != 0)
				{
					return -failedCountCmp;
				}

				var successfulCount1 = x.CheckResults.Count(c => c.IsSuccessful);
				var successfulCount2 = y.CheckResults.Count(c => c.IsSuccessful);

				return successfulCount1.CompareTo(successfulCount2);
			}
		}

		public IReadOnlyCollection<StudiedText> GetProblematicTexts(IEnumerable<StudiedText> studiedTexts)
		{
			// We consider text as problematic, if 3 or more of the last 5 checks were failed.
			return studiedTexts
				.Select(x => x.WithLimitedCheckResults(5))
				.Where(x => x.CheckResults.Count(y => y.IsFailed) >= 3)
				.Order(new ProblematicTextsCompare())
				.ToList();
		}
	}
}
