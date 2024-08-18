using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.ViewModels.Exercises
{
	public abstract class BasicExerciseViewModel : ObservableObject, IExerciseViewModel
	{
		private readonly ISystemClock systemClock;

		protected IExerciseResultService ExerciseResultService { get; }

		protected DateTimeOffset CurrentTimestamp => systemClock.Now;

		protected User User { get; private set; }

		private bool exerciseWasChecked;

		public bool ExerciseWasChecked
		{
			get => exerciseWasChecked;
			private set => SetProperty(ref exerciseWasChecked, value);
		}

		protected BasicExerciseViewModel(IExerciseResultService exerciseResultService, ISystemClock systemClock)
		{
			ExerciseResultService = exerciseResultService ?? throw new ArgumentNullException(nameof(exerciseResultService));
			this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		}

		protected void Load(User user)
		{
			User = user ?? throw new ArgumentNullException(nameof(user));
			ExerciseWasChecked = false;
		}

		protected abstract Task<BasicExerciseResult> CheckExerciseAndStoreResult(CancellationToken cancellationToken);

		public async Task<BasicExerciseResult> CheckExercise(CancellationToken cancellationToken)
		{
			var exerciseResult = await CheckExerciseAndStoreResult(cancellationToken);

			ExerciseWasChecked = true;

			return exerciseResult;
		}
	}
}
