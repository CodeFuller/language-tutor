using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.ViewModels.Exercises
{
	public class ProblematicInflectWordExerciseViewModel : BasicProblematicExerciseViewModel
	{
		public IReadOnlyCollection<ProblematicInflectWordExerciseResultViewModel> InflectWordExerciseResults { get; }

		protected override IEnumerable<BasicProblematicExerciseResultViewModel> ExerciseResults => InflectWordExerciseResults;

		public ProblematicInflectWordExerciseViewModel(InflectWordExercise exercise)
			: base(exercise.Description)
		{
			InflectWordExerciseResults = exercise.InflectWordExerciseResults
				.OrderBy(x => x.DateTime)
				.Select(x => new ProblematicInflectWordExerciseResultViewModel(x))
				.ToList();
		}
	}
}
