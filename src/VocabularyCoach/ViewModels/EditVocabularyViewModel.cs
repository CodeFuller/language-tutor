using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Events;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class EditVocabularyViewModel : IEditVocabularyViewModel
	{
		public ICommand GoToStartCommand { get; }

		public EditVocabularyViewModel(IMessenger messenger)
		{
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			GoToStartCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}
	}
}
