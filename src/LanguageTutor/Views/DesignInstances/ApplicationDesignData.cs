using System.Windows.Input;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.Views.DesignInstances
{
	internal sealed class ApplicationDesignData : IApplicationViewModel
	{
		public IStartPageViewModel StartPageViewModel { get; } = new StartPageDesignData();

		public IPracticeLanguageViewModel PracticeLanguageViewModel { get; } = new PracticeLanguageDesignData();

		public IPracticeResultsViewModel PracticeResultsViewModel { get; } = new PracticeResultsDesignData();

		public IEditDictionaryViewModel EditDictionaryViewModel { get; } = new EditDictionaryDesignData();

		public IPageViewModel CurrentPage => StartPageViewModel;

		public ICommand LoadCommand => null;
	}
}
