using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Data;
using LanguageTutor.ViewModels.Exercises;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.ViewModels
{
	internal class PerformExercisesViewModel : ObservableObject, IPerformExercisesViewModel
	{
		private class SetExerciseViewModelVisitor : IExerciseVisitor
		{
			private readonly PerformExercisesViewModel viewModel;

			public SetExerciseViewModelVisitor(PerformExercisesViewModel viewModel)
			{
				this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
			}

			public void VisitTranslateTextExercise(TranslateTextExercise exercise)
			{
				var exerciseViewModel = GetExerciseViewModel<TranslateTextExerciseViewModel>();

				exerciseViewModel.Load(viewModel.User, exercise);

				viewModel.CurrentExerciseViewModel = exerciseViewModel;
			}

			public void VisitInflectWordExercise(InflectWordExercise exercise)
			{
				var exerciseViewModel = GetExerciseViewModel<InflectWordExerciseViewModel>();

				exerciseViewModel.Load(viewModel.User, exercise);

				viewModel.CurrentExerciseViewModel = exerciseViewModel;
			}

			private TExerciseViewModel GetExerciseViewModel<TExerciseViewModel>()
				where TExerciseViewModel : IExerciseViewModel
			{
				return viewModel.exerciseViewModels.OfType<TExerciseViewModel>().Single();
			}
		}

		private readonly IReadOnlyCollection<IExerciseViewModel> exerciseViewModels;

		private readonly ITutorService tutorService;

		private readonly IMessenger messenger;

		private User User { get; set; }

		private Language StudiedLanguage { get; set; }

		private Language KnownLanguage { get; set; }

		private List<BasicExercise> Exercises { get; set; }

		public int NumberOfExercisesToPerform => Exercises.Count;

		private int numberOfPerformedExercises;

		public int NumberOfPerformedExercises
		{
			get => numberOfPerformedExercises;
			private set
			{
				SetProperty(ref numberOfPerformedExercises, value);
				OnPropertyChanged(nameof(ProgressInfo));
			}
		}

		public string ProgressInfo => $"{NumberOfPerformedExercises} / {NumberOfExercisesToPerform}";

		private IExerciseViewModel currentExerciseViewModel;

		public IExerciseViewModel CurrentExerciseViewModel
		{
			get => currentExerciseViewModel;
			private set => SetProperty(ref currentExerciseViewModel, value);
		}

		private int currentExerciseIndex;

		private int CurrentExerciseIndex
		{
			get => currentExerciseIndex;
			set
			{
				currentExerciseIndex = value;
				OnPropertyChanged(nameof(CanSwitchToNextExercise));
			}
		}

		private bool exerciseWasChecked;

		public bool ExerciseWasChecked
		{
			get => exerciseWasChecked;
			private set
			{
				SetProperty(ref exerciseWasChecked, value);

				OnPropertyChanged(nameof(CanSwitchToNextExercise));
			}
		}

		public bool CanSwitchToNextExercise => ExerciseWasChecked && CurrentExerciseIndex + 1 < NumberOfExercisesToPerform;

		private ExerciseResults ExerciseResults { get; set; }

		public ICommand CheckExerciseCommand { get; }

		public ICommand SwitchToNextExerciseCommand { get; }

		public ICommand FinishExercisesCommand { get; }

		public PerformExercisesViewModel(ITutorService tutorService, IMessenger messenger, IEnumerable<IExerciseViewModel> exerciseViewModels)
		{
			this.tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
			this.exerciseViewModels = exerciseViewModels?.ToList() ?? throw new ArgumentNullException(nameof(exerciseViewModels));

			CheckExerciseCommand = new AsyncRelayCommand(CheckExercise);
			SwitchToNextExerciseCommand = new RelayCommand(SwitchToNextExercise);
			FinishExercisesCommand = new RelayCommand(FinishExercises);

			messenger.Register<CheckOrSwitchToNextExerciseEventArgs>(this, (_, _) => CheckOrSwitchToNextExercise(CancellationToken.None));
		}

		public async Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			User = user;
			StudiedLanguage = studiedLanguage;
			KnownLanguage = knownLanguage;

			Exercises = (await tutorService.GetExercisesToPerform(User, studiedLanguage, knownLanguage, cancellationToken)).ToList();
			CurrentExerciseIndex = -1;
			NumberOfPerformedExercises = 0;

			ExerciseResults = new ExerciseResults();

			SwitchToNextExercise();
		}

		private async Task CheckExercise(CancellationToken cancellationToken)
		{
			var exerciseResult = await CurrentExerciseViewModel.CheckExercise(cancellationToken);

			ExerciseWasChecked = true;

			ExerciseResults.AddResult(exerciseResult.ResultType);

			++NumberOfPerformedExercises;
		}

		private void SwitchToNextExercise()
		{
			++CurrentExerciseIndex;
			if (CurrentExerciseIndex >= Exercises.Count)
			{
				FinishExercises();
				return;
			}

			var visitor = new SetExerciseViewModelVisitor(this);
			Exercises[CurrentExerciseIndex].Accept(visitor);

			ExerciseWasChecked = false;
		}

		private async void CheckOrSwitchToNextExercise(CancellationToken cancellationToken)
		{
			if (ExerciseWasChecked)
			{
				SwitchToNextExercise();
			}
			else
			{
				await CheckExercise(cancellationToken);
			}
		}

		private void FinishExercises()
		{
			messenger.Send(new SwitchToExerciseResultsPageEventArgs(StudiedLanguage, KnownLanguage, ExerciseResults));
		}
	}
}
