using System;
using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models.Exercises.Inflection;

namespace LanguageTutor.Models.Exercises
{
	public class InflectWordExerciseResult : BasicExerciseResult
	{
		public IReadOnlyCollection<InflectWordResult> FormResults { get; init; }

		public override ExerciseResultType ResultType
		{
			get
			{
				if (!FormResults.Any())
				{
					throw new InvalidOperationException("InflectWordExerciseResult does not contain form results");
				}

				if (FormResults.Any(x => x.ResultType == ExerciseResultType.Skipped))
				{
					return ExerciseResultType.Skipped;
				}

				if (FormResults.Any(x => x.ResultType == ExerciseResultType.Failed))
				{
					return ExerciseResultType.Failed;
				}

				return ExerciseResultType.Successful;
			}
		}
	}
}
