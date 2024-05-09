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
			TotalNumberOfTexts = 1234,
			TotalNumberOfLearnedTexts = 987,
			NumberOfProblematicTexts = 5,
			RestNumberOfTextsToPracticeToday = 135,
			RestNumberOfTextsToPracticeTodayIfNoLimit = 155,
			NumberOfTextsPracticedToday = 42,
			NumberOfTextsLearnedToday = 28,
		};

		public string RestNumberOfTextsToPracticeToday => "135 (155)";

		public bool LanguagesAreSelected => true;

		public bool HasTextsForPractice => true;

		public bool HasProblematicTexts => true;

		public ICommand PracticeLanguageCommand => null;

		public ICommand EditDictionaryCommand => null;

		public ICommand GoToProblematicTextsCommand => null;

		public ICommand ShowStatisticsChartCommand => null;

		public Task Load(User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
