using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.Views.DesignInstances
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
			NumberOfTextsPracticedToday = 42,
			NumberOfTextsLearnedToday = 28,
		};

		public bool HasTextsForPractice => true;

		public bool HasProblematicTexts => true;

		public ICommand PracticeVocabularyCommand => null;

		public ICommand EditVocabularyCommand => null;

		public ICommand GoToProblematicTextsCommand => null;

		public Task Load(User user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
