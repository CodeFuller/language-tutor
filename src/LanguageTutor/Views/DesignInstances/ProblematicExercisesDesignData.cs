using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.ViewModels.Exercises;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.Views.DesignInstances
{
	internal class ProblematicExercisesDesignData : IProblematicExercisesViewModel
	{
		public ObservableCollection<BasicProblematicExerciseViewModel> ProblematicExercises { get; } =
		[
			new ProblematicTranslateTextExerciseViewModel(new TranslateTextExercise(
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
			}),

			new ProblematicTranslateTextExerciseViewModel(new TranslateTextExercise(
			[
				new(DateTimeOffset.Now, ExerciseResultType.Skipped, null),
				new(DateTimeOffset.Now, ExerciseResultType.Skipped, null),
				new(DateTimeOffset.Now, ExerciseResultType.Failed, null),
				new(DateTimeOffset.Now, ExerciseResultType.Failed, null),
				new(DateTimeOffset.Now, ExerciseResultType.Failed, null),
			])
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "pewny",
				},

				SynonymsInKnownLanguage =
				[
					new LanguageText { Text = "уверенный" },
				],
			}),
		];

		public BasicProblematicExerciseViewModel SelectedExercise { get; set; }

		public ICommand GoToStartPageCommand => null;

		public ProblematicExercisesDesignData()
		{
			SelectedExercise = ProblematicExercises.First();
		}

		public Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
