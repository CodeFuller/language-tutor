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

		bool HasTextsForPractice { get; }

		ICommand PracticeVocabularyCommand { get; }

		ICommand EditVocabularyCommand { get; }

		Task Load(User user, CancellationToken cancellationToken);
	}
}
