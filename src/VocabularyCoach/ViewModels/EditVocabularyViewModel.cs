using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Events;
using VocabularyCoach.Extensions;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.ViewModels.Extensions;
using VocabularyCoach.ViewModels.Interfaces;
using static VocabularyCoach.ViewModels.Extensions.FocusHelpers;

namespace VocabularyCoach.ViewModels
{
	public class EditVocabularyViewModel : ObservableObject, IEditVocabularyViewModel
	{
		private readonly IEditVocabularyService editVocabularyService;

		public IEditLanguageTextViewModel EditTextInStudiedLanguageViewModel { get; }

		public IEditLanguageTextViewModel EditTextInKnownLanguageViewModel { get; }

		public ObservableCollection<TranslationViewModel> Translations { get; } = new();

		public ICommand SaveChangesCommand { get; }

		public ICommand ClearChangesCommand { get; }

		public ICommand GoToStartPageCommand { get; }

		public EditVocabularyViewModel(IEditVocabularyService editVocabularyService, IMessenger messenger,
			IEditLanguageTextViewModel editTextInStudiedLanguageViewModel, IEditLanguageTextViewModel textInKnownLanguageViewModel)
		{
			this.editVocabularyService = editVocabularyService ?? throw new ArgumentNullException(nameof(editVocabularyService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			if (Object.ReferenceEquals(editTextInStudiedLanguageViewModel, textInKnownLanguageViewModel))
			{
				throw new ArgumentException($"The same instance is injected for {nameof(editTextInStudiedLanguageViewModel)} and {nameof(textInKnownLanguageViewModel)}");
			}

			EditTextInStudiedLanguageViewModel = editTextInStudiedLanguageViewModel ?? throw new ArgumentNullException(nameof(editTextInStudiedLanguageViewModel));
			EditTextInKnownLanguageViewModel = textInKnownLanguageViewModel ?? throw new ArgumentNullException(nameof(textInKnownLanguageViewModel));

			SaveChangesCommand = new AsyncRelayCommand(SaveChanges);
			ClearChangesCommand = new RelayCommand(ClearFilledData);
			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));

			messenger.Register<EnterKeyPressedEventArgs>(this, (_, _) => ProcessEnterKeyPressed(CancellationToken.None));
			messenger.Register<EditedTextSpellCheckedEventArgs>(this, (_, _) => SetFocus(() => EditTextInKnownLanguageViewModel.TextIsFocused));
		}

		public async Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var translations = await editVocabularyService.GetTranslations(studiedLanguage, knownLanguage, cancellationToken);

			Translations.Clear();
			Translations.AddRange(translations.Select(x => new TranslationViewModel(x)).OrderBy(x => x.ToString()));

			await EditTextInStudiedLanguageViewModel.Load(studiedLanguage, requireSpellCheck: true, createPronunciationRecord: true, cancellationToken);
			await EditTextInKnownLanguageViewModel.Load(knownLanguage, requireSpellCheck: false, createPronunciationRecord: false, cancellationToken);

			ClearFilledData();
		}

		private async Task SaveChanges(CancellationToken cancellationToken)
		{
			EditTextInStudiedLanguageViewModel.ValidationIsEnabled = true;
			EditTextInKnownLanguageViewModel.ValidationIsEnabled = true;

			if (EditTextInStudiedLanguageViewModel.HasErrors || EditTextInKnownLanguageViewModel.HasErrors)
			{
				return;
			}

			var textInStudiedLanguage = await EditTextInStudiedLanguageViewModel.SaveChanges(cancellationToken);
			var textInKnownLanguage = await EditTextInKnownLanguageViewModel.SaveChanges(cancellationToken);

			var newTranslation = await editVocabularyService.AddTranslation(textInStudiedLanguage, textInKnownLanguage, cancellationToken);

			Translations.AddToSortedCollection(new TranslationViewModel(newTranslation));

			ClearFilledData();
		}

		private async void ProcessEnterKeyPressed(CancellationToken cancellationToken)
		{
			await SaveChanges(cancellationToken);
		}

		private void ClearFilledData()
		{
			EditTextInStudiedLanguageViewModel.ClearFilledData();
			EditTextInKnownLanguageViewModel.ClearFilledData();

			SetFocus(() => EditTextInStudiedLanguageViewModel.TextIsFocused);
		}
	}
}
