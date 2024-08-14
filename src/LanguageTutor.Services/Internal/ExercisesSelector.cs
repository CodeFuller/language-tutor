using System;
using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Models.Extensions;
using LanguageTutor.Services.Extensions;

namespace LanguageTutor.Services.Internal
{
	internal class ExercisesSelector : IExercisesSelector
	{
		private readonly INextExerciseDateProvider nextExerciseDateProvider;

		public ExercisesSelector(INextExerciseDateProvider nextExerciseDateProvider)
		{
			this.nextExerciseDateProvider = nextExerciseDateProvider ?? throw new ArgumentNullException(nameof(nextExerciseDateProvider));
		}

		public IReadOnlyCollection<BasicExercise> SelectExercisesToPerform(DateOnly date, IEnumerable<BasicExercise> exercises, int dailyLimit)
		{
			var exercisesList = exercises.ToList();

			var numberOfAlreadyCompletedExercisesForDate = exercisesList.Count(x => x.SortedResults.Any(y => y.DateTime.ToDateOnly() == date));
			var numberOfRestExercisesForDay = dailyLimit - numberOfAlreadyCompletedExercisesForDate;

			if (numberOfRestExercisesForDay <= 0)
			{
				return Array.Empty<BasicExercise>();
			}

			var untouchedExercises = exercisesList
				.Where(x => !x.SortedResults.Any())
				.OrderBy(x => x.CreationTimestamp)
				.ToList();

			var suitableExercises = new List<BasicExercise>();

			var suitableTouchedExercises = exercisesList
				.Where(x => x.SortedResults.Any())
				.Select(x => new
				{
					Exercise = x,
					NextExerciseDateTime = nextExerciseDateProvider.GetNextExerciseDate(x),
				})
				.Where(x => x.NextExerciseDateTime <= date)
				.GroupBy(x => x.NextExerciseDateTime, x => x.Exercise)
				.OrderBy(x => x.Key)
				.SelectMany(x => x.Randomize());

			// The logic is the following:
			//
			//   If there are a lot of untouched exercises (i.e. this is a new user, who has a lot of exercises to learn),
			//   then we add untouched exercises gradually, only if she has learned previous exercises already.
			//
			//   If there are not too much untouched exercises (i.e. this is an old user and just new exercises were added recently),
			//   then we include all untouched exercises.
			if (untouchedExercises.Count > dailyLimit)
			{
				suitableExercises.AddRange(suitableTouchedExercises);

				if (suitableExercises.Count < numberOfRestExercisesForDay)
				{
					// Taking first untouched exercises.
					var suitableUntouchedExercises = untouchedExercises
						.Take(numberOfRestExercisesForDay - suitableExercises.Count);

					suitableExercises.AddRange(suitableUntouchedExercises);
				}
			}
			else
			{
				suitableExercises.AddRange(untouchedExercises);
				suitableExercises.AddRange(suitableTouchedExercises);
			}

			return suitableExercises
				.Take(numberOfRestExercisesForDay)
				.Randomize()
				.ToList();
		}
	}
}
