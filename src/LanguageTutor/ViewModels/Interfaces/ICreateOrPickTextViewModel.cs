using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;

namespace LanguageTutor.ViewModels.Interfaces
{
	public interface ICreateOrPickTextViewModel : IBasicEditTextViewModel
	{
		ObservableCollection<LanguageTextViewModel> ExistingTexts { get; }

		LanguageTextViewModel SelectedText { get; set; }

		Task Load(Language language, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken);
	}
}
