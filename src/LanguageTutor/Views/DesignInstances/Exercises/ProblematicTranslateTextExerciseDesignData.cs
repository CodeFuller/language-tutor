using System;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.ViewModels.Exercises;

namespace LanguageTutor.Views.DesignInstances.Exercises
{
	internal class ProblematicTranslateTextExerciseDesignData : ProblematicTranslateTextExerciseViewModel
	{
		private static TranslateTextExercise Exercise { get; } = new(
		[
			new(new DateTimeOffset(2023, 08, 17, 08, 16, 41, TimeSpan.FromHours(2)), ExerciseResultType.Skipped, null),
			new(new DateTimeOffset(2023, 08, 19, 11, 01, 09, TimeSpan.FromHours(2)), ExerciseResultType.Failed, "źmęczony"),
			new(new DateTimeOffset(2023, 08, 20, 10, 48, 05, TimeSpan.FromHours(2)), ExerciseResultType.Successful, null),
		])
		{
			TextInStudiedLanguage = new LanguageText
			{
				Text = "zmęczony",
			},

			SynonymsInKnownLanguage =
			[
				new LanguageText { Text = "уставший" },
				new LanguageText { Text = "усталый" },
			],
		};

		public ProblematicTranslateTextExerciseDesignData()
			: base(Exercise)
		{
		}
	}
}
