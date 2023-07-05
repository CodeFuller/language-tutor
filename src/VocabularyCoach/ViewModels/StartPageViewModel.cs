using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Events;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class StartPageViewModel : IStartPageViewModel
	{
		public ICommand StudyVocabularyCommand { get; }

		public ICommand EditVocabularyCommand { get; }

		public StartPageViewModel(IMessenger messenger)
		{
			StudyVocabularyCommand = new RelayCommand(() => messenger.Send(new SwitchToStudyVocabularyPageEventArgs()));
			EditVocabularyCommand = new RelayCommand(() => messenger.Send(new SwitchToEditVocabularyPageEventArgs()));
		}
	}
}
