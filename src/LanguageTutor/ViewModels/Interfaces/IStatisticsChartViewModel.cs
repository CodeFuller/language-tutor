using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;

namespace LanguageTutor.ViewModels.Interfaces
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
