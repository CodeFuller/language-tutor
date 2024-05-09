using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LanguageTutor.Models;
using LanguageTutor.ViewModels;
using LanguageTutor.ViewModels.ContextMenu;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.Views.DesignInstances
{
	internal sealed class EditDictionaryDesignData : IEditDictionaryViewModel
	{
		public IBasicEditTextViewModel CurrentTextInStudiedLanguageViewModel { get; } = new CreateOrPickTextInStudiedLanguageDesignData();

		public IBasicEditTextViewModel CurrentTextInKnownLanguageViewModel { get; } = new CreateOrPickTextInKnownLanguageDesignData();

		public bool EditTextInStudiedLanguageIsEnabled => true;

		public bool EditTextInKnownLanguageIsEnabled => true;

		public string TranslationFilter { get; set; }

		public IReadOnlyCollection<TranslationViewModel> FilteredTranslations => DesignData.Translations.Select(x => new TranslationViewModel(x)).ToList();

		public TranslationViewModel SelectedTranslation { get; set; }

		public IAsyncRelayCommand SaveChangesCommand => null;

		public ICommand ClearChangesCommand => null;

		public ICommand GoToStartPageCommand => null;

		public Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ContextMenuItem> GetContextMenuItemsForSelectedTranslation()
		{
			throw new NotImplementedException();
		}
	}
}
