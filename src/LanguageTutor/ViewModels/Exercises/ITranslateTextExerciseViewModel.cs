using System.Windows.Input;
using LanguageTutor.Models.Exercises;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.ViewModels.Exercises
{
	public interface ITranslateTextExerciseViewModel : IExerciseViewModel
	{
		TranslateTextExercise Exercise { get; }

		string DisplayedTextInKnownLanguage { get; }

		string HintForOtherSynonyms { get; }

		bool PronunciationRecordExists { get; }

		bool TypedTextIsFocused { get; }

		string TypedText { get; set; }

		bool ExerciseWasPerformedCorrectly { get; }

		bool ExerciseWasPerformedIncorrectly { get; }

		public ICommand NextStepCommand { get; }

		ICommand PlayPronunciationRecordCommand { get; }
	}
}
