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
			new() { Date = new DateOnly(2023, 07, 10), TotalNumberOfExercises = 40, TotalNumberOfLearnedExercises = 0 },
			new() { Date = new DateOnly(2023, 07, 11), TotalNumberOfExercises = 100, TotalNumberOfLearnedExercises = 0 },
			new() { Date = new DateOnly(2023, 07, 12), TotalNumberOfExercises = 132, TotalNumberOfLearnedExercises = 0 },
			new() { Date = new DateOnly(2023, 07, 13), TotalNumberOfExercises = 150, TotalNumberOfLearnedExercises = 0 },
			new() { Date = new DateOnly(2023, 07, 14), TotalNumberOfExercises = 152, TotalNumberOfLearnedExercises = 0 },
			new() { Date = new DateOnly(2023, 07, 15), TotalNumberOfExercises = 201, TotalNumberOfLearnedExercises = 32 },
			new() { Date = new DateOnly(2023, 07, 16), TotalNumberOfExercises = 251, TotalNumberOfLearnedExercises = 81 },
			new() { Date = new DateOnly(2023, 07, 17), TotalNumberOfExercises = 251, TotalNumberOfLearnedExercises = 106 },
			new() { Date = new DateOnly(2023, 07, 18), TotalNumberOfExercises = 251, TotalNumberOfLearnedExercises = 128 },
			new() { Date = new DateOnly(2023, 07, 19), TotalNumberOfExercises = 251, TotalNumberOfLearnedExercises = 134 },
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
