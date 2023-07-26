using System;
using System.Collections.Generic;
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
using VocabularyCoach.ViewModels.ContextMenu;
using VocabularyCoach.ViewModels.Extensions;
using VocabularyCoach.ViewModels.Interfaces;
using static VocabularyCoach.ViewModels.Extensions.FocusHelpers;

namespace VocabularyCoach.ViewModels
{
	public class EditVocabularyViewModel : ObservableObject, IEditVocabularyViewModel
	{
		private enum EditMode
		{
			None,
			NewTranslation,
			EditTextInStudiedLanguage,
			EditTextInKnownLanguage,
		}

		private readonly IEditVocabularyService editVocabularyService;

		public IEditLanguageTextViewModel EditTextInStudiedLanguageViewModel { get; }

		public IEditLanguageTextViewModel EditTextInKnownLanguageViewModel { get; }

		public bool EditTextInStudiedLanguageIsEnabled => CurrentEditMode is EditMode.NewTranslation or EditMode.EditTextInStudiedLanguage;

		public bool EditTextInKnownLanguageIsEnabled => CurrentEditMode is EditMode.NewTranslation or EditMode.EditTextInKnownLanguage;

		private EditMode editMode;

		private EditMode CurrentEditMode
		{
			get => editMode;
			set
			{
				editMode = value;

				OnPropertyChanged(nameof(EditTextInStudiedLanguageIsEnabled));
				OnPropertyChanged(nameof(EditTextInKnownLanguageIsEnabled));
			}
		}

		private Language StudiedLanguage { get; set; }

		private Language KnownLanguage { get; set; }

		public ObservableCollection<TranslationViewModel> Translations { get; } = new();

		public TranslationViewModel SelectedTranslation { get; set; }

		public ICommand SaveChangesCommand { get; }

		public ICommand ClearChangesCommand { get; }

		public ICommand GoToStartPageCommand { get; }

		public IEnumerable<ContextMenuItem> ContextMenuItems
		{
			get
			{
				var selectedTranslation = SelectedTranslation;

				if (selectedTranslation == null)
				{
					yield break;
				}

				var languageText1 = selectedTranslation.LanguageText1;
				var languageText2 = selectedTranslation.LanguageText2;

				yield return new ContextMenuItem
				{
					Header = $"Edit Text '{languageText1.TextWithNote}'",
					Command = new AsyncRelayCommand(cancellationToken => EditLanguageTextInStudiedLanguage(languageText1.LanguageText, cancellationToken)),
				};

				yield return new ContextMenuItem
				{
					Header = $"Edit Text '{languageText2.TextWithNote}'",
					Command = new AsyncRelayCommand(cancellationToken => EditLanguageTextInKnownLanguage(languageText2.LanguageText, cancellationToken)),
				};

				yield return new ContextMenuItem
				{
					Header = "Delete Translation",
					Command = new AsyncRelayCommand(cancellationToken => DeleteTranslation(selectedTranslation.Translation, cancellationToken)),
				};

				yield return new ContextMenuItem
				{
					Header = $"Delete Text '{languageText1.TextWithNote}' and {GetNumberOfAffectedTranslations(languageText1.LanguageText)} translation(s)",
					Command = new AsyncRelayCommand(cancellationToken => DeleteLanguageText(languageText1.LanguageText, cancellationToken)),
				};

				yield return new ContextMenuItem
				{
					Header = $"Delete Text '{languageText2.TextWithNote}' and {GetNumberOfAffectedTranslations(languageText2.LanguageText)} translation(s)",
					Command = new AsyncRelayCommand(cancellationToken => DeleteLanguageText(languageText2.LanguageText, cancellationToken)),
				};

				yield return new ContextMenuItem
				{
					Header = $"Delete Both Texts and {GetNumberOfAffectedTranslations(languageText1.LanguageText, languageText2.LanguageText)} translation(s)",
					Command = new AsyncRelayCommand(cancellationToken => DeleteLanguageTexts(languageText1.LanguageText, languageText2.LanguageText, cancellationToken)),
				};
			}
		}

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
			StudiedLanguage = studiedLanguage;
			KnownLanguage = knownLanguage;

			await ReloadData(cancellationToken);

			CurrentEditMode = EditMode.NewTranslation;
		}

		private async Task ReloadData(CancellationToken cancellationToken)
		{
			var translations = await editVocabularyService.GetTranslations(StudiedLanguage, KnownLanguage, cancellationToken);

			Translations.Clear();
			Translations.AddRange(translations.Select(x => new TranslationViewModel(x)).OrderBy(x => x.ToString()));

			await EditTextInStudiedLanguageViewModel.LoadForNewText(StudiedLanguage, requireSpellCheck: true, createPronunciationRecord: true, cancellationToken);
			await EditTextInKnownLanguageViewModel.LoadForNewText(KnownLanguage, requireSpellCheck: false, createPronunciationRecord: false, cancellationToken);

			ClearFilledData();
		}

		private async Task SaveChanges(CancellationToken cancellationToken)
		{
			switch (CurrentEditMode)
			{
				case EditMode.NewTranslation:
					await SaveChangesForNewTranslation(cancellationToken);
					break;

				case EditMode.EditTextInStudiedLanguage:
					await SaveChangesForLanguageText(EditTextInStudiedLanguageViewModel, cancellationToken);
					break;

				case EditMode.EditTextInKnownLanguage:
					await SaveChangesForLanguageText(EditTextInKnownLanguageViewModel, cancellationToken);
					break;

				default:
					throw new NotSupportedException($"Saving changes for mode {CurrentEditMode} is not supported");
			}
		}

		private async Task SaveChangesForNewTranslation(CancellationToken cancellationToken)
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

		private async Task SaveChangesForLanguageText(IEditLanguageTextViewModel editLanguageTextViewModel, CancellationToken cancellationToken)
		{
			editLanguageTextViewModel.ValidationIsEnabled = true;

			if (editLanguageTextViewModel.HasErrors)
			{
				return;
			}

			await editLanguageTextViewModel.SaveChanges(cancellationToken);

			// We reload data so that all translations with edited text are updated.
			await ReloadData(cancellationToken);
		}

		private async void ProcessEnterKeyPressed(CancellationToken cancellationToken)
		{
			await SaveChanges(cancellationToken);
		}

		private void ClearFilledData()
		{
			EditTextInStudiedLanguageViewModel.ClearFilledData();
			EditTextInKnownLanguageViewModel.ClearFilledData();

			CurrentEditMode = EditMode.NewTranslation;

			SetFocus(() => EditTextInStudiedLanguageViewModel.TextIsFocused);
		}

		private async Task EditLanguageTextInStudiedLanguage(LanguageText languageText, CancellationToken cancellationToken)
		{
			await EditTextInStudiedLanguageViewModel.LoadForEditText(languageText, requireSpellCheck: true, createPronunciationRecord: true, cancellationToken);

			EditTextInKnownLanguageViewModel.ClearFilledData();

			CurrentEditMode = EditMode.EditTextInStudiedLanguage;
		}

		private async Task EditLanguageTextInKnownLanguage(LanguageText languageText, CancellationToken cancellationToken)
		{
			await EditTextInKnownLanguageViewModel.LoadForEditText(languageText, requireSpellCheck: false, createPronunciationRecord: false, cancellationToken);

			EditTextInStudiedLanguageViewModel.ClearFilledData();

			CurrentEditMode = EditMode.EditTextInKnownLanguage;
		}

		private async Task DeleteTranslation(Translation translation, CancellationToken cancellationToken)
		{
			await editVocabularyService.DeleteTranslation(translation, cancellationToken);

			await ReloadData(cancellationToken);
		}

		private async Task DeleteLanguageText(LanguageText languageText, CancellationToken cancellationToken)
		{
			await editVocabularyService.DeleteLanguageText(languageText, cancellationToken);

			await ReloadData(cancellationToken);
		}

		private async Task DeleteLanguageTexts(LanguageText languageText1, LanguageText languageText2, CancellationToken cancellationToken)
		{
			await editVocabularyService.DeleteLanguageText(languageText1, cancellationToken);
			await editVocabularyService.DeleteLanguageText(languageText2, cancellationToken);

			await ReloadData(cancellationToken);
		}

		private int GetNumberOfAffectedTranslations(params LanguageText[] languageTexts)
		{
			return Translations
				.Select(x => x.Translation)
				.Count(translation => languageTexts.Any(text => text.Id == translation.Text1.Id || text.Id == translation.Text2.Id));
		}
	}
}
