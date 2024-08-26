using System.Collections.Generic;
using System.Linq;

namespace LanguageTutor.ViewModels.Exercises
{
	public abstract class BasicProblematicExerciseViewModel
	{
		public string Title { get; }

		public IEnumerable<BasicProblematicExerciseResultViewModel> OrderedExerciseResults => ExerciseResults.OrderBy(x => x.DateTime);

		protected abstract IEnumerable<BasicProblematicExerciseResultViewModel> ExerciseResults { get; }

		protected BasicProblematicExerciseViewModel(string title)
		{
			Title = title;
		}
	}
}
