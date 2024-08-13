using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Interfaces;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LanguageTutor.ViewModels
{
	internal class StatisticsChartViewModel : IStatisticsChartViewModel
	{
		private readonly ITutorService tutorService;

		public IReadOnlyCollection<ISeries> Series { get; private set; }

		public IReadOnlyCollection<ICartesianAxis> XAxes { get; private set; }

		public IReadOnlyCollection<ICartesianAxis> YAxes { get; private set; }

		public ICommand GoToStartPageCommand { get; }

		public StatisticsChartViewModel(ITutorService tutorService, IMessenger messenger)
		{
			this.tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var statistics = await tutorService.GetUserStatisticsHistory(user, studiedLanguage, knownLanguage, cancellationToken);

			Series = GetSeries(statistics);
			XAxes = GetXAxes(statistics);
			YAxes = GetYAxes();
		}

		internal static IReadOnlyCollection<ISeries> GetSeries(IReadOnlyCollection<UserStatisticsData> statistics)
		{
			return new[]
			{
				GetNumberOfExercisesSeries(statistics.Select(x => x.TotalNumberOfExercises).ToList()),
				GetNumberOfLearnedExercisesSeries(statistics.Select(x => x.TotalNumberOfLearnedExercises).ToList()),
			};
		}

		private static ISeries GetNumberOfExercisesSeries(IEnumerable<int> values)
		{
			return GetSeries("Number of exercises", values, SKColors.Blue);
		}

		private static ISeries GetNumberOfLearnedExercisesSeries(IEnumerable<int> values)
		{
			return GetSeries("Number of learned exercises", values, SKColors.Green);
		}

		private static ISeries GetSeries(string name, IEnumerable<int> values, SKColor color)
		{
			return new LineSeries<int, SquareGeometry>
			{
				Name = name,
				Values = values,
				LineSmoothness = 1,
				GeometrySize = 0,
				Fill = null,
				MiniatureShapeSize = 10,
				Stroke = new SolidColorPaint(color, 2),
				GeometryStroke = new SolidColorPaint(color),
				GeometryFill = new SolidColorPaint(color),
			};
		}

		internal static IReadOnlyCollection<Axis> GetXAxes(IEnumerable<UserStatisticsData> statistics)
		{
			return new[]
			{
				new Axis
				{
					Labels = statistics.Select(x => x.Date.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture)).ToList(),

					TextSize = 12,
					LabelsPaint = new SolidColorPaint(SKColors.Black),
					SeparatorsPaint = new SolidColorPaint(SKColors.Black) { StrokeThickness = 0.5f },
				},
			};
		}

		internal static IReadOnlyCollection<Axis> GetYAxes()
		{
			return new[]
			{
				new Axis
				{
					TextSize = 16,
					LabelsPaint = new SolidColorPaint(SKColors.Black),
					SeparatorsPaint = new SolidColorPaint(SKColors.Black) { StrokeThickness = 0.5f },
					Labeler = x => x.ToString("N0", CultureInfo.InvariantCulture),
				},
			};
		}
	}
}
