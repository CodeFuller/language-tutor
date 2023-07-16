using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels.Data;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.Views.DesignInstances
{
	internal sealed class PracticeResultsDesignData : IPracticeResultsViewModel
	{
		public string PracticedTextsStatistics => "100";

		public string CorrectTextStatistics => "95 (95.0%)";

		public string IncorrectTextStatistics => "3 (3.0%)";

		public string SkippedTextStatistics => "2 (2.0%)";

		public ICommand GoToStartPageCommand => null;

		public Task Load(User user, Language studiedLanguage, Language knownLanguage, PracticeResults results, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
