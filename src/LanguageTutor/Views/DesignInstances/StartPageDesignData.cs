using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.Views.DesignInstances
{
	internal sealed class StartPageDesignData : IStartPageViewModel
	{
		public ObservableCollection<Language> AvailableLanguages { get; } = new()
		{
			DesignData.StudiedLanguage,
			DesignData.KnownLanguage,
		};

		public Language SelectedStudiedLanguage { get; set; } = DesignData.StudiedLanguage;

		public Language SelectedKnownLanguage { get; set; } = DesignData.KnownLanguage;

		public UserStatisticsData UserStatistics { get; } = new()
		{
			Date = new DateOnly(2023, 08, 19),
			TotalNumberOfExercises = 1234,
			TotalNumberOfLearnedExercises = 987,
			NumberOfProblematicExercises = 5,
			RestNumberOfExercisesToPerformToday = 135,
			RestNumberOfExercisesToPerformTodayIfNoLimit = 155,
			NumberOfExercisesPerformedToday = 42,
			NumberOfExercisesLearnedToday = 28,
		};

		public string RestNumberOfExercisesToPerformToday => "135 (155)";

		public bool LanguagesAreSelected => true;

		public bool HasExercisesToPerform => true;

		public bool HasProblematicExercises => true;

		public ICommand PerformExercisesCommand => null;

		public ICommand EditDictionaryCommand => null;

		public ICommand GoToProblematicExercisesCommand => null;

		public ICommand ShowStatisticsChartCommand => null;

		public Task Load(User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
