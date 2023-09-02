using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using VocabularyCoach.Events;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	internal class StatisticsChartViewModel : IStatisticsChartViewModel
	{
		private readonly IVocabularyService vocabularyService;

		public IReadOnlyCollection<ISeries> Series { get; private set; }

		public IReadOnlyCollection<ICartesianAxis> XAxes { get; private set; }

		public IReadOnlyCollection<ICartesianAxis> YAxes { get; private set; }

		public ICommand GoToStartPageCommand { get; }

		public StatisticsChartViewModel(IVocabularyService vocabularyService, IMessenger messenger)
		{
			this.vocabularyService = vocabularyService ?? throw new ArgumentNullException(nameof(vocabularyService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var statistics = await vocabularyService.GetUserStatisticsHistory(user, studiedLanguage, knownLanguage, cancellationToken);

			Series = GetSeries(statistics);
			XAxes = GetXAxes(statistics);
			YAxes = GetYAxes();
		}

		internal static IReadOnlyCollection<ISeries> GetSeries(IReadOnlyCollection<UserStatisticsData> statistics)
		{
			return new[]
			{
				GetNumberOfTextsSeries(statistics.Select(x => x.TotalNumberOfTexts).ToList()),
				GetNumberOfLearnedTextsSeriesTemplate(statistics.Select(x => x.TotalNumberOfLearnedTexts).ToList()),
			};
		}

		private static ISeries GetNumberOfTextsSeries(IEnumerable<int> values)
		{
			return GetSeries("Number of texts", values, SKColors.Blue);
		}

		private static ISeries GetNumberOfLearnedTextsSeriesTemplate(IEnumerable<int> values)
		{
			return GetSeries("Number of learned texts", values, SKColors.Green);
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

		internal static IReadOnlyCollection<Axis> GetXAxes(IEnumerable<UserStatisticsData> dates)
		{
			return new[]
			{
				new Axis
				{
					Labels = dates.Select(x => x.Date.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture)).ToList(),

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
				},
			};
		}
	}
}
