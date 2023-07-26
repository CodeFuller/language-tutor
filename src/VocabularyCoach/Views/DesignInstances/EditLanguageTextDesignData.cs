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
	internal abstract class EditLanguageTextDesignData : IEditLanguageTextViewModel
	{
		public bool NewTextIsEdited => true;

		public bool ExistingTextIsEdited => !NewTextIsEdited;

		public abstract Language Language { get; }

		public abstract bool RequireSpellCheck { get; }

		public abstract bool CreatePronunciationRecord { get; }

		public abstract ObservableCollection<LanguageTextViewModel> ExistingTexts { get; }

		public bool TextIsFocused { get; set; }

		public abstract string Text { get; set; }

		public abstract bool TextWasSpellChecked { get; }

		public bool TextIsFilled => true;

		public LanguageTextViewModel SelectedText { get; set; }

		public bool ExistingTextIsSelected => false;

		public abstract string Note { get; set; }

		public bool ValidationIsEnabled { get; set; }

		public bool HasErrors => false;

		public ICommand SpellCheckTextCommand => null;

		public ICommand PlayPronunciationRecordCommand => null;

		public ICommand ProcessEnterKeyCommand => null;

		public Task LoadForNewText(Language language, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task LoadForEditText(LanguageText editedLanguageText, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken)
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
	}
}
