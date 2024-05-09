using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FluentAssertions.Equivalency;
using LanguageTutor.Models;
using LanguageTutor.ViewModels;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.UnitTests.Helpers
{
	internal sealed class CreateOrPickTextViewModelData : ICreateOrPickTextViewModel
	{
		public Language Language { get; set; }

		public bool RequireSpellCheck { get; set; }

		public bool CreatePronunciationRecord { get; set; }

		public string Text { get; set; }

		public bool TextIsFocused { get; set; }

		public bool TextWasSpellChecked { get; set; }

		public bool TextIsFilled { get; set; }

		public string Note { get; set; }

		public bool AllowNoteEdit { get; set; }

		public bool ValidationIsEnabled { get; set; }

		public bool HasErrors { get; set; }

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
		{
			add { }
			remove { }
		}

		public IAsyncRelayCommand SpellCheckTextCommand => null;

		public IAsyncRelayCommand ProcessPastedTextCommand => null;

		public IAsyncRelayCommand PlayPronunciationRecordCommand => null;

		public ICommand ProcessEnterKeyCommand => null;

		public static Func<EquivalencyAssertionOptions<CreateOrPickTextViewModelData>, EquivalencyAssertionOptions<CreateOrPickTextViewModelData>> ExcludingCommands
		{
			get
			{
				return x => x
					.Excluding(y => y.SpellCheckTextCommand)
					.Excluding(y => y.ProcessPastedTextCommand)
					.Excluding(y => y.PlayPronunciationRecordCommand)
					.Excluding(y => y.ProcessEnterKeyCommand);
			}
		}

		public ObservableCollection<LanguageTextViewModel> ExistingTexts { get; set; }

		public LanguageTextViewModel SelectedText { get; set; }

		public Task Load(Language language, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<LanguageText> SaveChanges(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public void ClearFilledData()
		{
			throw new NotImplementedException();
		}

		public IEnumerable GetErrors(string propertyName)
		{
			throw new NotImplementedException();
		}
	}
}
