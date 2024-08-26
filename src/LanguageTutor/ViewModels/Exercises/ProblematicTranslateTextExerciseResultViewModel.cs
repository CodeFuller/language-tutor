using LanguageTutor.Models.Exercises;

namespace LanguageTutor.ViewModels.Exercises
{
    public class ProblematicTranslateTextExerciseResultViewModel : BasicProblematicExerciseResultViewModel
	{
		public string TypedText { get; }

		public ProblematicTranslateTextExerciseResultViewModel(TranslateTextExerciseResult exerciseResult)
			: base(exerciseResult)
		{
			TypedText = exerciseResult.TypedText;
		}
	}
}
