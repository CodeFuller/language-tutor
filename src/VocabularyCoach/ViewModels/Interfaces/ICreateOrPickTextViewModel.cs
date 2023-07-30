using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface ICreateOrPickTextViewModel : IBasicEditTextViewModel
	{
		public ObservableCollection<LanguageTextViewModel> ExistingTexts { get; }

		public LanguageTextViewModel SelectedText { get; set; }

		Task Load(Language language, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken);
	}
}
