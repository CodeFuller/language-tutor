using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Events;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class ApplicationViewModel : ObservableObject
	{
		private readonly IMessenger messenger;

		public IStartPageViewModel StartPageViewModel { get; }

		public IStudyVocabularyViewModel StudyVocabularyViewModel { get; }

		public IEditVocabularyViewModel EditVocabularyViewModel { get; }

		private IPageViewModel currentPage;

		public IPageViewModel CurrentPage
		{
			get => currentPage;
			private set => SetProperty(ref currentPage, value);
		}

		public ApplicationViewModel(IStartPageViewModel startPageViewModel, IStudyVocabularyViewModel studyVocabularyViewModel,
			IEditVocabularyViewModel editVocabularyViewModel, IMessenger messenger)
		{
			StartPageViewModel = startPageViewModel ?? throw new ArgumentNullException(nameof(startPageViewModel));
			StudyVocabularyViewModel = studyVocabularyViewModel ?? throw new ArgumentNullException(nameof(studyVocabularyViewModel));
			EditVocabularyViewModel = editVocabularyViewModel ?? throw new ArgumentNullException(nameof(editVocabularyViewModel));

			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			this.messenger.Register<SwitchToStartPageEventArgs>(this, (_, _) => CurrentPage = StartPageViewModel);
			this.messenger.Register<SwitchToStudyVocabularyPageEventArgs>(this, (_, _) => CurrentPage = StudyVocabularyViewModel);
			this.messenger.Register<SwitchToEditVocabularyPageEventArgs>(this, (_, _) => CurrentPage = EditVocabularyViewModel);

			CurrentPage = StartPageViewModel;
		}
	}
}
