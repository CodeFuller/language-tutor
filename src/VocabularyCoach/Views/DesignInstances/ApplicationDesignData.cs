using System.Windows.Input;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.Views.DesignInstances
{
	internal sealed class ApplicationDesignData : IApplicationViewModel
	{
		public IStartPageViewModel StartPageViewModel { get; } = new StartPageDesignData();

		public IPracticeVocabularyViewModel PracticeVocabularyViewModel { get; } = new PracticeVocabularyDesignData();

		public IPracticeResultsViewModel PracticeResultsViewModel { get; } = new PracticeResultsDesignData();

		public IEditVocabularyViewModel EditVocabularyViewModel { get; } = new EditVocabularyDesignData();

		public IPageViewModel CurrentPage => StartPageViewModel;

		public ICommand LoadCommand => null;
	}
}
