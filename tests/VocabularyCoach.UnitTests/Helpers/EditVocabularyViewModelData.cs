using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FluentAssertions.Equivalency;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels;
using VocabularyCoach.ViewModels.ContextMenu;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.UnitTests.Helpers
{
	internal sealed class EditVocabularyViewModelData : IEditVocabularyViewModel
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

		public static Func<EquivalencyAssertionOptions<EditVocabularyViewModelData>, EquivalencyAssertionOptions<EditVocabularyViewModelData>> ExcludingCommands
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
