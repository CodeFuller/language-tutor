using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FluentAssertions.Equivalency;
using LanguageTutor.Models;
using LanguageTutor.ViewModels;
using LanguageTutor.ViewModels.ContextMenu;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.UnitTests.Helpers
{
	internal sealed class EditDictionaryViewModelData : IEditDictionaryViewModel
	{
		public IBasicEditTextViewModel CurrentTextInStudiedLanguageViewModel { get; set; }

		public IBasicEditTextViewModel CurrentTextInKnownLanguageViewModel { get; set; }

		public bool EditTextInStudiedLanguageIsEnabled { get; set; }

		public bool EditTextInKnownLanguageIsEnabled { get; set; }

		public string TranslationFilter { get; set; }

		public IReadOnlyCollection<TranslationViewModel> FilteredTranslations { get; set; }

		public TranslationViewModel SelectedTranslation { get; set; }

		public IAsyncRelayCommand SaveChangesCommand => null;

		public ICommand ClearChangesCommand => null;

		public ICommand GoToStartPageCommand => null;

		public static Func<EquivalencyOptions<EditDictionaryViewModelData>, EquivalencyOptions<EditDictionaryViewModelData>> ExcludingCommands
		{
			get
			{
				return x => x
					.Excluding(y => y.SaveChangesCommand)
					.Excluding(y => y.ClearChangesCommand)
					.Excluding(y => y.GoToStartPageCommand);
			}
		}

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
