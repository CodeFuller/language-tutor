using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.ViewModels;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.Views.DesignInstances
{
	internal class ProblematicExercisesDesignData : IProblematicExercisesViewModel
	{
		public ObservableCollection<ProblematicExerciseViewModel> ProblematicExercises { get; } = new()
		{
			new ProblematicExerciseViewModel(new TranslateTextExercise(new TranslateTextExerciseResult[]
			{
				new()
				{
					ResultType = ExerciseResultType.Skipped,
					DateTime = new DateTimeOffset(2023, 08, 17, 08, 16, 41, TimeSpan.FromHours(2)),
				},

				new()
				{
					ResultType = ExerciseResultType.Failed,
					DateTime = new DateTimeOffset(2023, 08, 19, 11, 01, 09, TimeSpan.FromHours(2)),
					TypedText = "źmęczony",
				},

				new()
				{
					ResultType = ExerciseResultType.Successful,
					DateTime = new DateTimeOffset(2023, 08, 20, 10, 48, 05, TimeSpan.FromHours(2)),
				},
			})
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "zmęczony",
				},

				SynonymsInKnownLanguage = new[]
				{
					new LanguageText { Text = "уставший" },
					new LanguageText { Text = "усталый" },
				},
			}),

			new ProblematicExerciseViewModel(new TranslateTextExercise(new TranslateTextExerciseResult[]
			{
				new() { ResultType = ExerciseResultType.Skipped },
				new() { ResultType = ExerciseResultType.Skipped },
				new() { ResultType = ExerciseResultType.Failed },
				new() { ResultType = ExerciseResultType.Failed },
				new() { ResultType = ExerciseResultType.Failed },
			})
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "pewny",
				},

				SynonymsInKnownLanguage = new[]
				{
					new LanguageText { Text = "уверенный" },
				},
			}),
		};

		public ProblematicExerciseViewModel SelectedExercise { get; set; }

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
