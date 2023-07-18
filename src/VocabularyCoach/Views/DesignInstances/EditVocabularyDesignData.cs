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
		public IEditLanguageTextViewModel EditTextInStudiedLanguageViewModel { get; } = new EditTextInStudiedLanguageDesignData();

		public IEditLanguageTextViewModel EditTextInKnownLanguageViewModel { get; } = new EditTextInKnownLanguageDesignData();

		public ObservableCollection<LanguageTextViewModel> ExistingTextsInStudiedLanguage { get; } = new()
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

		public ICommand SaveChangesCommand => null;

		public ICommand ClearChangesCommand => null;

		public ICommand GoToStartPageCommand => null;

		public Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
