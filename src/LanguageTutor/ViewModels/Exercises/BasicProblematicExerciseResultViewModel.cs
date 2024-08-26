using System;
using System.Globalization;
using System.Windows.Media;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using MaterialDesignThemes.Wpf;

namespace LanguageTutor.ViewModels.Exercises
{
	public abstract class BasicProblematicExerciseResultViewModel
	{
		public PackIconKind IconKind { get; }

		public Brush IconColor { get; }

		public DateTimeOffset DateTime { get; }

		public string DateTimeString => DateTime.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture);

		protected BasicProblematicExerciseResultViewModel(BasicExerciseResult exerciseResult)
		{
			switch (exerciseResult.ResultType)
			{
				case ExerciseResultType.Successful:
					IconKind = PackIconKind.CheckCircle;
					IconColor = Brushes.Green;
					break;

				case ExerciseResultType.Failed:
					IconKind = PackIconKind.CancelCircle;
					IconColor = Brushes.Orange;
					break;

				case ExerciseResultType.Skipped:
					IconKind = PackIconKind.DoNotDisturbOn;
					IconColor = Brushes.Red;
					break;

				default:
					throw new NotSupportedException($"Exercise result type is not supported: {exerciseResult.ResultType}");
			}

			DateTime = exerciseResult.DateTime;
		}
	}
}
