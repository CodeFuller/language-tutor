using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageTutor.Models.Exercises
{
	public class TranslateTextExercise : BasicExercise
	{
		private readonly List<TranslateTextExerciseResult> results;

		public LanguageText TextInStudiedLanguage { get; init; }

		public IReadOnlyCollection<LanguageText> OtherSynonymsInStudiedLanguage { get; init; }

		public IReadOnlyCollection<LanguageText> SynonymsInKnownLanguage { get; init; }

		public override DateTimeOffset CreationTimestamp => TextInStudiedLanguage.CreationTimestamp;

		protected override IEnumerable<BasicExerciseResult> Results => results;

		public TranslateTextExercise(IEnumerable<TranslateTextExerciseResult> results)
		{
			this.results = results?.ToList() ?? throw new ArgumentNullException(nameof(results));
		}

		protected override BasicExercise WithLimitedResults(IEnumerable<BasicExerciseResult> limitedResults)
		{
			return new TranslateTextExercise(limitedResults.Cast<TranslateTextExerciseResult>())
			{
				TextInStudiedLanguage = TextInStudiedLanguage,
				OtherSynonymsInStudiedLanguage = OtherSynonymsInStudiedLanguage,
				SynonymsInKnownLanguage = SynonymsInKnownLanguage,
			};
		}

		public TranslateTextExerciseResult Check(string typedText, DateTimeOffset timestamp)
		{
			var resultType = GetResultType(typedText);

			var exerciseResult = new TranslateTextExerciseResult
			{
				DateTime = timestamp,
				ResultType = resultType,
				TypedText = resultType == ExerciseResultType.Failed ? typedText : null,
			};

			results.Add(exerciseResult);

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
