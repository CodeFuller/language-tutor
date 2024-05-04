using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IStartPageViewModel : IPageViewModel
	{
		ObservableCollection<Language> AvailableLanguages { get; }

		Language SelectedStudiedLanguage { get; set; }

		Language SelectedKnownLanguage { get; set; }

		UserStatisticsData UserStatistics { get; }

		string RestNumberOfTextsToPracticeToday { get; }

		bool LanguagesAreSelected { get; }

		bool HasTextsForPractice { get; }

		bool HasProblematicTexts { get; }

		ICommand PracticeVocabularyCommand { get; }

		ICommand EditVocabularyCommand { get; }

		ICommand GoToProblematicTextsCommand { get; }

		ICommand ShowStatisticsChartCommand { get; }

		Task Load(User user, CancellationToken cancellationToken);
	}
}
