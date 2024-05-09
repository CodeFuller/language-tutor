using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.ViewModels;
using LanguageTutor.ViewModels.Interfaces;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;

namespace LanguageTutor.Views.DesignInstances
{
	internal class StatisticsChartDesignData : IStatisticsChartViewModel
	{
		private static readonly IReadOnlyCollection<UserStatisticsData> Statistics = new UserStatisticsData[]
		{
			new() { Date = new DateOnly(2023, 07, 10), TotalNumberOfTexts = 40, TotalNumberOfLearnedTexts = 0 },
			new() { Date = new DateOnly(2023, 07, 11), TotalNumberOfTexts = 100, TotalNumberOfLearnedTexts = 0 },
			new() { Date = new DateOnly(2023, 07, 12), TotalNumberOfTexts = 132, TotalNumberOfLearnedTexts = 0 },
			new() { Date = new DateOnly(2023, 07, 13), TotalNumberOfTexts = 150, TotalNumberOfLearnedTexts = 0 },
			new() { Date = new DateOnly(2023, 07, 14), TotalNumberOfTexts = 152, TotalNumberOfLearnedTexts = 0 },
			new() { Date = new DateOnly(2023, 07, 15), TotalNumberOfTexts = 201, TotalNumberOfLearnedTexts = 32 },
			new() { Date = new DateOnly(2023, 07, 16), TotalNumberOfTexts = 251, TotalNumberOfLearnedTexts = 81 },
			new() { Date = new DateOnly(2023, 07, 17), TotalNumberOfTexts = 251, TotalNumberOfLearnedTexts = 106 },
			new() { Date = new DateOnly(2023, 07, 18), TotalNumberOfTexts = 251, TotalNumberOfLearnedTexts = 128 },
			new() { Date = new DateOnly(2023, 07, 19), TotalNumberOfTexts = 251, TotalNumberOfLearnedTexts = 134 },
		};

		public IReadOnlyCollection<ISeries> Series { get; } = StatisticsChartViewModel.GetSeries(Statistics);

		public IReadOnlyCollection<ICartesianAxis> XAxes { get; } = StatisticsChartViewModel.GetXAxes(Statistics);

		public IReadOnlyCollection<ICartesianAxis> YAxes { get; } = StatisticsChartViewModel.GetYAxes();

		public ICommand GoToStartPageCommand => null;

		public Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
