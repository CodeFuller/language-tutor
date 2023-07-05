using System.Windows.Input;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IStudyVocabularyViewModel : IPageViewModel
	{
		ICommand FinishStudyCommand { get; }
	}
}
