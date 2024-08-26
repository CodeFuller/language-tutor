using System;
using System.Collections.Generic;

namespace LanguageTutor.Models.Exercises
{
	public class TranslateTextExercise : BasicExercise<TranslateTextExerciseResult>
	{
		public LanguageText TextInStudiedLanguage { get; init; }

		public IReadOnlyCollection<LanguageText> OtherSynonymsInStudiedLanguage { get; init; }

		public IReadOnlyCollection<LanguageText> SynonymsInKnownLanguage { get; init; }

		public override DateTimeOffset CreationTimestamp => TextInStudiedLanguage.CreationTimestamp;

		public IEnumerable<TranslateTextExerciseResult> TranslateTextExerciseResults => StronglyTypedSortedResults;

		public TranslateTextExercise(IEnumerable<TranslateTextExerciseResult> results)
			: base(results)
		{
		}

		protected override BasicExercise<TranslateTextExerciseResult> WithLimitedResults(IEnumerable<TranslateTextExerciseResult> limitedResults)
		{
			return new TranslateTextExercise(limitedResults)
			{
				TextInStudiedLanguage = TextInStudiedLanguage,
				OtherSynonymsInStudiedLanguage = OtherSynonymsInStudiedLanguage,
				SynonymsInKnownLanguage = SynonymsInKnownLanguage,
			};
		}

		public TranslateTextExerciseResult Check(string typedText, DateTimeOffset timestamp)
		{
			var resultType = GetResultType(typedText);

			var exerciseResult = new TranslateTextExerciseResult(timestamp, resultType, resultType == ExerciseResultType.Failed ? typedText : null);

			AddResult(exerciseResult);

			return exerciseResult;
		}

		private ExerciseResultType GetResultType(string typedText)
		{
			if (String.IsNullOrEmpty(typedText))
			{
				return ExerciseResultType.Skipped;
			}

			return String.Equals(TextInStudiedLanguage.Text, typedText, StringComparison.Ordinal)
				? ExerciseResultType.Successful
				: ExerciseResultType.Failed;
		}

		public override void Accept(IExerciseVisitor visitor)
		{
			visitor.VisitTranslateTextExercise(this);
		}
	}
}
