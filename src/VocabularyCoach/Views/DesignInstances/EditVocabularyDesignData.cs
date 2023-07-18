using System;
using System.Collections.ObjectModel;
using System.Linq;
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

		public ObservableCollection<TranslationViewModel> Translations { get; } = new(DesignData.Translations.Select(x => new TranslationViewModel(x)));

		public ICommand SaveChangesCommand => null;

		public ICommand ClearChangesCommand => null;

		public ICommand GoToStartPageCommand => null;

		public Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
