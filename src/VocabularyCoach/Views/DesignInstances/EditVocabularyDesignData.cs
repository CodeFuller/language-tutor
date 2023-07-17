using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.Views.DesignInstances
{
	internal sealed class EditVocabularyDesignData : IEditVocabularyViewModel
	{
		public Language StudiedLanguage => DesignData.StudiedLanguage;

		public Language KnownLanguage => DesignData.KnownLanguage;

		public ObservableCollection<LanguageTextViewModel> TextsInStudiedLanguage { get; } = new()
		{
			new LanguageTextViewModel(
				new LanguageText
				{
					Id = new ItemId("1"),
					Language = DesignData.StudiedLanguage,
					Text = "dziękuję",
				}),

			new LanguageTextViewModel(
				new LanguageText
				{
					Id = new ItemId("2"),
					Language = DesignData.StudiedLanguage,
					Text = "proszę",
				}),
		};

		public ObservableCollection<LanguageTextViewModel> TextsInKnownLanguage { get; } = new()
		{
			new LanguageTextViewModel(
				new LanguageText
				{
					Id = new ItemId("3"),
					Language = DesignData.KnownLanguage,
					Text = "спасибо",
				}),

			new LanguageTextViewModel(
				new LanguageText
				{
					Id = new ItemId("4"),
					Language = DesignData.KnownLanguage,
					Text = "пожалуйста",
					Note = "ответ на спасибо",
				}),
		};

		public bool TextInStudiedLanguageIsFocused => false;

		public string TextInStudiedLanguage { get; set; } = "samochód";

		public bool TextInStudiedLanguageWasChecked => true;

		public bool TextInStudiedLanguageIsFilled => true;

		public bool TextInKnownLanguageIsFocused => false;

		public string TextInKnownLanguage { get; set; } = "автомобиль";

		public LanguageTextViewModel SelectedTextInKnownLanguage { get; set; }

		public bool ExistingTextInKnownLanguageIsSelected => false;

		public string NoteInKnownLanguage { get; set; } = "машина";

		public ICommand CheckTextCommand => null;

		public ICommand PlayPronunciationRecordCommand => null;

		public ICommand SaveChangesCommand => null;

		public ICommand ClearChangesCommand => null;

		public ICommand GoToStartPageCommand => null;

		public Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
