using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.Views.DesignInstances
{
	internal abstract class BasicCreateOrPickTextDesignData : ICreateOrPickTextViewModel
	{
		public abstract Language Language { get; }

		public abstract bool RequireSpellCheck { get; }

		public abstract bool CreatePronunciationRecord { get; }

		public abstract string Text { get; set; }

		public bool TextIsFocused { get; set; }

		public abstract bool TextWasSpellChecked { get; }

		public abstract bool TextIsFilled { get; }

		public abstract string Note { get; set; }

		public bool AllowTextEdit => true;

		public bool AllowNoteEdit => true;

		public abstract ObservableCollection<LanguageTextViewModel> ExistingTexts { get; }

		public LanguageTextViewModel SelectedText { get; set; }

		public bool ValidationIsEnabled { get; set; }

		public bool HasErrors => false;

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
		{
			add { }
			remove { }
		}

		public IAsyncRelayCommand SpellCheckTextCommand => null;

		public IAsyncRelayCommand ProcessPastedTextCommand => null;

		public IAsyncRelayCommand PlayPronunciationRecordCommand => null;

		public ICommand ProcessEnterKeyCommand => null;

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
