using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IStatisticsChartViewModel : IPageViewModel
	{
		IReadOnlyCollection<ISeries> Series { get; }

		IReadOnlyCollection<ICartesianAxis> XAxes { get; }

		IReadOnlyCollection<ICartesianAxis> YAxes { get; }

		ICommand GoToStartPageCommand { get; }

		Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
