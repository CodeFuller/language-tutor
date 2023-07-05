using System.Windows.Input;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IStartPageViewModel : IPageViewModel
	{
		public ICommand StudyVocabularyCommand { get; }

		public ICommand EditVocabularyCommand { get; }
	}
}
