using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IEditLanguageTextViewModel
	{
		Language Language { get; }

		bool RequireSpellCheck { get; }

		bool CreatePronunciationRecord { get; }

		ObservableCollection<LanguageTextViewModel> ExistingTexts { get; }

		bool TextIsFocused { get; set; }

		string Text { get; set; }

		bool TextWasSpellChecked { get; }

		bool TextIsFilled { get; }

		LanguageTextViewModel SelectedText { get; set; }

		bool ExistingTextIsSelected { get; }

		string Note { get; set; }

		bool ValidationIsEnabled { get; set; }

		bool HasErrors { get; }

		ICommand SpellCheckTextCommand { get; }

		ICommand PlayPronunciationRecordCommand { get; }

		ICommand ProcessEnterKeyCommand { get; }

		Task Load(Language language, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken);

		Task<LanguageText> SaveChanges(CancellationToken cancellationToken);

		void ClearFilledData();
	}
}
