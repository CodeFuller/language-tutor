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

		string RestNumberOfExercisesToPerformToday { get; }

		bool LanguagesAreSelected { get; }

		bool HasExercisesToPerform { get; }

		bool HasProblematicExercises { get; }

		bool CanEditExercises { get; }

		ICommand PerformExercisesCommand { get; }

		ICommand EditDictionaryCommand { get; }

		ICommand EditExercisesCommand { get; }

		ICommand GoToProblematicExercisesCommand { get; }

		ICommand ShowStatisticsChartCommand { get; }

		Task Load(User user, CancellationToken cancellationToken);
	}
}
