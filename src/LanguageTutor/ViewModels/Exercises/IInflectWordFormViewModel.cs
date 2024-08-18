using System.Windows.Input;

namespace LanguageTutor.ViewModels.Exercises
{
	public interface IInflectWordFormViewModel
	{
		string FormHint { get; }

		string CorrectWordForm { get; }

		string TypedWordForm { get; set; }

		bool TypedWordIsFocused { get; set; }

		bool WordFormWasChecked { get; }

		bool WordFormIsTypedCorrectly { get; }

		bool WordFormIsTypedIncorrectly { get; }

		ICommand NextStepCommand { get; }

		void Load();
	}
}
