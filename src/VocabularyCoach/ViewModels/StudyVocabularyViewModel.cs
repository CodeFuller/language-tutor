using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Events;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class StudyVocabularyViewModel : IStudyVocabularyViewModel
	{
		public ICommand FinishStudyCommand { get; }

		public StudyVocabularyViewModel(IMessenger messenger)
		{
			FinishStudyCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}
	}
}
