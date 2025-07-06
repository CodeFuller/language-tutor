using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Models.Exercises.Inflection;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Extensions;

namespace LanguageTutor.ViewModels.Exercises
{
	public class InflectWordExerciseViewModel : BasicExerciseViewModel, IInflectWordExerciseViewModel
	{
		private readonly IMessenger messenger;

		private InflectWordExercise exercise;

		public InflectWordExercise Exercise
		{
			get => exercise;
			private set
			{
				SetProperty(ref exercise, value);
				OnPropertyChanged(nameof(Description));
			}
		}

		public string Description => exercise.Description;

		private List<InflectWordFormViewModel> WordFormViewModelList { get; set; }

		public ObservableCollection<IInflectWordFormViewModel> WordFormViewModels { get; } = new();

		public InflectWordExerciseViewModel(IExerciseResultService exerciseResultService, IMessenger messenger, ISystemClock systemClock)
			: base(exerciseResultService, systemClock)
		{
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			messenger.Register<NextStepWithinExerciseEventArgs>(this, (_, _) => SwitchToNextFormOrCheckExercise());
		}

		public void Load(User user, InflectWordExercise exercise)
		{
			Load(user);

			Exercise = exercise ?? throw new ArgumentNullException(nameof(exercise));

			WordFormViewModelList = exercise.WordForms
				.Select(x => new InflectWordFormViewModel(messenger, x.FormHint, x.WordForm))
				.ToList();

			WordFormViewModels.Clear();
			WordFormViewModels.AddRange(WordFormViewModelList);

			WordFormViewModels[0].Load();
		}

		private void SwitchToNextFormOrCheckExercise()
		{
			if (ExerciseWasChecked)
			{
				messenger.Send(new CheckOrSwitchToNextExerciseEventArgs());
				return;
			}

			var focusedWordForm = WordFormViewModels.SingleOrDefault(x => x.TypedWordIsFocused);
			var index = WordFormViewModels.IndexOf(focusedWordForm);
			if (index == -1)
			{
				return;
			}

			if (index == WordFormViewModels.Count - 1)
			{
				messenger.Send(new CheckOrSwitchToNextExerciseEventArgs());
				return;
			}

			WordFormViewModels[index + 1].Load();
		}

		protected override async Task<BasicExerciseResult> CheckExerciseAndStoreResult(CancellationToken cancellationToken)
		{
			var typedWordForms = WordFormViewModels
				.Select(x => new InflectWordForm
				{
					FormHint = x.FormHint,
					WordForm = x.TypedWordForm,
				})
				.ToList();

			var exerciseResult = Exercise.Check(typedWordForms, CurrentTimestamp);

			foreach (var (formResult, i) in exerciseResult.FormResults.Select((formResult, i) => (formResult, i)))
			{
				WordFormViewModelList[i].WordFormWasChecked = true;

				var isSuccessful = formResult.ResultType == ExerciseResultType.Successful;
				WordFormViewModelList[i].WordFormIsTypedCorrectly = isSuccessful;
				WordFormViewModelList[i].WordFormIsTypedIncorrectly = !isSuccessful;
			}

			await ExerciseResultService.AddInflectWordExerciseResult(User, Exercise, exerciseResult, cancellationToken);

			return exerciseResult;
		}
	}
}
