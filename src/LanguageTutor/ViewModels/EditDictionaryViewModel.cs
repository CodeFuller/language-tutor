using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.Models;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Collections;
using LanguageTutor.ViewModels.ContextMenu;
using LanguageTutor.ViewModels.Extensions;
using LanguageTutor.ViewModels.Interfaces;
using static LanguageTutor.ViewModels.Extensions.FocusHelpers;

namespace LanguageTutor.ViewModels
{
	public class EditDictionaryViewModel : ObservableObject, IEditDictionaryViewModel
	{
		private enum EditMode
		{
			None,
			NewTranslation,
			EditTextInStudiedLanguage,
			EditTextInKnownLanguage,
		}

		private readonly IDictionaryService dictionaryService;

		private readonly ICreateOrPickTextViewModel createOrPickTextInStudiedLanguageViewModel;

		private readonly ICreateOrPickTextViewModel createOrPickTextInKnownLanguageViewModel;

		private readonly IEditExistingTextViewModel editExistingTextInStudiedLanguageViewModel;

		private readonly IEditExistingTextViewModel editExistingTextInKnownLanguageViewModel;

		private IBasicEditTextViewModel currentTextInStudiedLanguageViewModel;

		public IBasicEditTextViewModel CurrentTextInStudiedLanguageViewModel
		{
			get => currentTextInStudiedLanguageViewModel;
			private set => SetProperty(ref currentTextInStudiedLanguageViewModel, value);
		}

		private IBasicEditTextViewModel currentTextInKnownLanguageViewModel;

		public IBasicEditTextViewModel CurrentTextInKnownLanguageViewModel
		{
			get => currentTextInKnownLanguageViewModel;
			private set => SetProperty(ref currentTextInKnownLanguageViewModel, value);
		}

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

		private string translationFilter;

		public string TranslationFilter
		{
			get => translationFilter;
			set
			{
				SetProperty(ref translationFilter, value);

				OnPropertyChanged(nameof(FilteredTranslations));
			}
		}

		private SmartObservableCollection<TranslationViewModel> Translations { get; } = new();

		public IReadOnlyCollection<TranslationViewModel> FilteredTranslations => Translations.Where(TranslationMatchesFilter).ToList();

		public TranslationViewModel SelectedTranslation { get; set; }

		public IAsyncRelayCommand SaveChangesCommand { get; }

		public ICommand ClearChangesCommand { get; }

		public ICommand GoToStartPageCommand { get; }

		public EditDictionaryViewModel(IDictionaryService dictionaryService, IMessenger messenger,
			ICreateOrPickTextViewModel createOrPickTextInStudiedLanguageViewModel, ICreateOrPickTextViewModel createOrPickTextInKnownLanguageViewModel,
			IEditExistingTextViewModel editExistingTextInStudiedLanguageViewModel, IEditExistingTextViewModel editExistingTextInKnownLanguageViewModel)
		{
			this.dictionaryService = dictionaryService ?? throw new ArgumentNullException(nameof(dictionaryService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			if (Object.ReferenceEquals(createOrPickTextInStudiedLanguageViewModel, createOrPickTextInKnownLanguageViewModel))
			{
				throw new ArgumentException($"The same instance is injected for {nameof(createOrPickTextInStudiedLanguageViewModel)} and {nameof(createOrPickTextInKnownLanguageViewModel)}");
			}

			if (Object.ReferenceEquals(editExistingTextInStudiedLanguageViewModel, editExistingTextInKnownLanguageViewModel))
			{
				throw new ArgumentException($"The same instance is injected for {nameof(editExistingTextInStudiedLanguageViewModel)} and {nameof(editExistingTextInKnownLanguageViewModel)}");
			}

			this.createOrPickTextInStudiedLanguageViewModel = createOrPickTextInStudiedLanguageViewModel ?? throw new ArgumentNullException(nameof(createOrPickTextInStudiedLanguageViewModel));
			this.createOrPickTextInKnownLanguageViewModel = createOrPickTextInKnownLanguageViewModel ?? throw new ArgumentNullException(nameof(createOrPickTextInKnownLanguageViewModel));
			this.editExistingTextInStudiedLanguageViewModel = editExistingTextInStudiedLanguageViewModel ?? throw new ArgumentNullException(nameof(editExistingTextInStudiedLanguageViewModel));
			this.editExistingTextInKnownLanguageViewModel = editExistingTextInKnownLanguageViewModel ?? throw new ArgumentNullException(nameof(editExistingTextInKnownLanguageViewModel));

			SaveChangesCommand = new AsyncRelayCommand(SaveChanges);
			ClearChangesCommand = new RelayCommand(ClearFilledData);
			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));

			messenger.Register<EnterKeyPressedEventArgs>(this, (_, _) => ProcessEnterKeyPressed(CancellationToken.None));
			messenger.Register<EditedTextSpellCheckedEventArgs>(this, (_, _) => ProcessEditedTextSpellCheckedEvent());

			Translations.CollectionChanged += (_, _) => OnPropertyChanged(nameof(FilteredTranslations));
		}

		public async Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			StudiedLanguage = studiedLanguage;
			KnownLanguage = knownLanguage;

			await ReloadData(cancellationToken);

			TranslationFilter = String.Empty;
			SelectedTranslation = null;
		}

		private async Task ReloadData(CancellationToken cancellationToken)
		{
			var translations = await dictionaryService.GetTranslations(StudiedLanguage, KnownLanguage, cancellationToken);

			Translations.Clear();
			Translations.AddRange(translations.Select(x => new TranslationViewModel(x)).OrderBy(x => x.ToString()));

			// We are doing this just in case (and for UT).
			// Selected item will be cleared anyway because the list of items is reloaded.
			SelectedTranslation = null;

			await createOrPickTextInStudiedLanguageViewModel.Load(StudiedLanguage, requireSpellCheck: true, createPronunciationRecord: true, cancellationToken);
			await createOrPickTextInKnownLanguageViewModel.Load(KnownLanguage, requireSpellCheck: false, createPronunciationRecord: false, cancellationToken);

			SwitchToNewTranslationMode();
		}

		private async Task SaveChanges(CancellationToken cancellationToken)
		{
			switch (CurrentEditMode)
			{
				case EditMode.NewTranslation:
					await SaveChangesForNewTranslation(cancellationToken);
					break;

				case EditMode.EditTextInStudiedLanguage:
					await SaveChangesForLanguageText(editExistingTextInStudiedLanguageViewModel, cancellationToken);
					break;

				case EditMode.EditTextInKnownLanguage:
					await SaveChangesForLanguageText(editExistingTextInKnownLanguageViewModel, cancellationToken);
					break;

				default:
					throw new NotSupportedException($"Saving changes for mode {CurrentEditMode} is not supported");
			}
		}

		private async Task SaveChangesForNewTranslation(CancellationToken cancellationToken)
		{
			createOrPickTextInStudiedLanguageViewModel.ValidationIsEnabled = true;
			createOrPickTextInKnownLanguageViewModel.ValidationIsEnabled = true;

			if (createOrPickTextInStudiedLanguageViewModel.HasErrors || createOrPickTextInKnownLanguageViewModel.HasErrors)
			{
				return;
			}

			var textInStudiedLanguage = await createOrPickTextInStudiedLanguageViewModel.SaveChanges(cancellationToken);
			var textInKnownLanguage = await createOrPickTextInKnownLanguageViewModel.SaveChanges(cancellationToken);

			var newTranslation = await dictionaryService.AddTranslation(textInStudiedLanguage, textInKnownLanguage, cancellationToken);

			Translations.AddToSortedCollection(new TranslationViewModel(newTranslation));

			createOrPickTextInStudiedLanguageViewModel.ClearFilledData();
			createOrPickTextInKnownLanguageViewModel.ClearFilledData();
			SetFocus(() => createOrPickTextInStudiedLanguageViewModel.TextIsFocused);
		}

		private async Task SaveChangesForLanguageText(IBasicEditTextViewModel editLanguageTextViewModel, CancellationToken cancellationToken)
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

		private void ProcessEditedTextSpellCheckedEvent()
		{
			if (CurrentEditMode == EditMode.NewTranslation)
			{
				SetFocus(() => createOrPickTextInKnownLanguageViewModel.TextIsFocused);
			}
		}

		private void ClearFilledData()
		{
			createOrPickTextInStudiedLanguageViewModel.ClearFilledData();
			createOrPickTextInKnownLanguageViewModel.ClearFilledData();

			SwitchToNewTranslationMode();
		}

		// The caller of this method should take care of calling Load() or ClearFilledData() on view models.
		private void SwitchToNewTranslationMode()
		{
			CurrentTextInStudiedLanguageViewModel = createOrPickTextInStudiedLanguageViewModel;
			CurrentTextInKnownLanguageViewModel = createOrPickTextInKnownLanguageViewModel;

			CurrentEditMode = EditMode.NewTranslation;

			SetFocus(() => createOrPickTextInStudiedLanguageViewModel.TextIsFocused);
		}

		public IEnumerable<ContextMenuItem> GetContextMenuItemsForSelectedTranslation()
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

		private async Task EditLanguageTextInStudiedLanguage(LanguageText languageText, CancellationToken cancellationToken)
		{
			await editExistingTextInStudiedLanguageViewModel.Load(languageText, requireSpellCheck: true, createPronunciationRecord: true, cancellationToken);
			CurrentTextInStudiedLanguageViewModel = editExistingTextInStudiedLanguageViewModel;

			createOrPickTextInKnownLanguageViewModel.ClearFilledData();
			CurrentTextInKnownLanguageViewModel = createOrPickTextInKnownLanguageViewModel;

			CurrentEditMode = EditMode.EditTextInStudiedLanguage;
		}

		private async Task EditLanguageTextInKnownLanguage(LanguageText languageText, CancellationToken cancellationToken)
		{
			await editExistingTextInKnownLanguageViewModel.Load(languageText, requireSpellCheck: false, createPronunciationRecord: false, cancellationToken);

			createOrPickTextInStudiedLanguageViewModel.ClearFilledData();
			CurrentTextInStudiedLanguageViewModel = createOrPickTextInStudiedLanguageViewModel;

			editExistingTextInStudiedLanguageViewModel.ClearFilledData();
			CurrentTextInKnownLanguageViewModel = editExistingTextInKnownLanguageViewModel;

			CurrentEditMode = EditMode.EditTextInKnownLanguage;
		}

		private async Task DeleteTranslation(Translation translation, CancellationToken cancellationToken)
		{
			await dictionaryService.DeleteTranslation(translation, cancellationToken);

			await ReloadData(cancellationToken);
		}

		private async Task DeleteLanguageText(LanguageText languageText, CancellationToken cancellationToken)
		{
			await dictionaryService.DeleteLanguageText(languageText, cancellationToken);

			await ReloadData(cancellationToken);
		}

		private async Task DeleteLanguageTexts(LanguageText languageText1, LanguageText languageText2, CancellationToken cancellationToken)
		{
			await dictionaryService.DeleteLanguageText(languageText1, cancellationToken);
			await dictionaryService.DeleteLanguageText(languageText2, cancellationToken);

			await ReloadData(cancellationToken);
		}

		private int GetNumberOfAffectedTranslations(params LanguageText[] languageTexts)
		{
			return Translations
				.Select(x => x.Translation)
				.Count(translation => languageTexts.Any(text => text.Id == translation.Text1.Id || text.Id == translation.Text2.Id));
		}

		private bool TranslationMatchesFilter(TranslationViewModel translation)
		{
			return String.IsNullOrEmpty(TranslationFilter) || translation.ToString().Contains(TranslationFilter, LanguageTextComparison.IgnoreCase);
		}
	}
}
