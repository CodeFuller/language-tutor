using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IBasicEditTextViewModel : INotifyDataErrorInfo
	{
		Language Language { get; }

		bool RequireSpellCheck { get; }

		bool CreatePronunciationRecord { get; }

		string Text { get; set; }

		bool TextIsFocused { get; set; }

		bool TextWasSpellChecked { get; }

		bool TextIsFilled { get; }

		string Note { get; set; }

		bool AllowTextEdit { get; }

		bool AllowNoteEdit { get; }

		bool ValidationIsEnabled { get; set; }

		IAsyncRelayCommand SpellCheckTextCommand { get; }

		IAsyncRelayCommand PlayPronunciationRecordCommand { get; }

		ICommand ProcessEnterKeyCommand { get; }

		Task<LanguageText> SaveChanges(CancellationToken cancellationToken);

		void ClearFilledData();
	}
}
