namespace LanguageTutor.Models.Exercises
{
	public interface IExerciseVisitor
	{
		void VisitTranslateTextExercise(TranslateTextExercise exercise);

		void VisitInflectWordExercise(InflectWordExercise exercise);
	}
}
