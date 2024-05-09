using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;

namespace LanguageTutor.ViewModels.Interfaces
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

		ICommand PracticeLanguageCommand { get; }

		ICommand EditDictionaryCommand { get; }

		ICommand GoToProblematicTextsCommand { get; }

		ICommand ShowStatisticsChartCommand { get; }

		Task Load(User user, CancellationToken cancellationToken);
	}
}
