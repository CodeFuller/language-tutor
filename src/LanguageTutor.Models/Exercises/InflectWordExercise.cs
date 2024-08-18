using System;
using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models.Exercises.Inflection;

namespace LanguageTutor.Models.Exercises
{
	public class InflectWordExercise : BasicExercise<InflectWordExerciseResult>
	{
		public override DateTimeOffset CreationTimestamp { get; }

		public string Description { get; }

		public string BaseForm { get; }

		public IReadOnlyCollection<InflectWordForm> WordForms { get; }

		public InflectWordExercise(DateTimeOffset creationTimestamp, string description, string baseForm,
			IReadOnlyCollection<InflectWordForm> wordForms, IEnumerable<InflectWordExerciseResult> results)
			: base(results)
		{
			CreationTimestamp = creationTimestamp;
			Description = description;
			BaseForm = baseForm;
			WordForms = wordForms ?? throw new ArgumentNullException(nameof(wordForms));
		}

		protected override BasicExercise<InflectWordExerciseResult> WithLimitedResults(IEnumerable<InflectWordExerciseResult> limitedResults)
		{
			return new InflectWordExercise(CreationTimestamp, Description, BaseForm, WordForms, limitedResults);
		}

		public InflectWordExerciseResult Check(IEnumerable<InflectWordForm> typedWordForms, DateTimeOffset timestamp)
		{
			var typedFormsDictionary = typedWordForms.ToDictionary(x => x.FormHint, x => x.WordForm);

			var exerciseResult = new InflectWordExerciseResult
			{
				DateTime = timestamp,
				FormResults = WordForms.Select(x => GetWordFormResult(x, typedFormsDictionary[x.FormHint])).ToList(),
			};

			AddResult(exerciseResult);

			return exerciseResult;
		}

		private static InflectWordResult GetWordFormResult(InflectWordForm wordForm, string typedWordForm)
		{
			var resultType = GetResultType(wordForm.WordForm, typedWordForm);

			return new InflectWordResult
			{
				FormHint = wordForm.FormHint,
				ResultType = resultType,
				TypedWord = resultType == ExerciseResultType.Failed ? typedWordForm : null,
			};
		}

		private static ExerciseResultType GetResultType(string expectedWordForm, string typedWordForm)
		{
			if (String.IsNullOrEmpty(typedWordForm))
			{
				return ExerciseResultType.Skipped;
			}

			return String.Equals(expectedWordForm, typedWordForm, StringComparison.Ordinal)
				? ExerciseResultType.Successful
				: ExerciseResultType.Failed;
		}

		public override void Accept(IExerciseVisitor visitor)
		{
			visitor.VisitInflectWordExercise(this);
		}
	}
}
