using System;
using System.Globalization;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels
{
	public class ProblematicTextCheckResultViewModel
	{
		public PackIconKind IconKind { get; }

		public Brush IconColor { get; }

		public string DateTime { get; }

		public string TypedText { get; }

		public ProblematicTextCheckResultViewModel(CheckResult checkResult)
		{
			switch (checkResult.CheckResultType)
			{
				case CheckResultType.Ok:
					IconKind = PackIconKind.CheckCircle;
					IconColor = Brushes.Green;
					break;

				case CheckResultType.Misspelled:
					IconKind = PackIconKind.CancelCircle;
					IconColor = Brushes.Orange;
					break;

				case CheckResultType.Skipped:
					IconKind = PackIconKind.DoNotDisturbOn;
					IconColor = Brushes.Red;
					break;

				default:
					throw new NotSupportedException($"Check result type is not supported: {checkResult.CheckResultType}");
			}

			DateTime = checkResult.DateTime.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture);
			TypedText = checkResult.TypedText;
		}
	}
}
