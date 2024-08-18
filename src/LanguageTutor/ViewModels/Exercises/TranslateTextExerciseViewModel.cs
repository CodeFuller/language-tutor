using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.Interfaces;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Extensions;
using static LanguageTutor.ViewModels.Extensions.FocusHelpers;

namespace LanguageTutor.ViewModels.Exercises
{
	public class TranslateTextExerciseViewModel : BasicExerciseViewModel, ITranslateTextExerciseViewModel
	{
		private readonly ITutorService tutorService;

		private readonly IPronunciationRecordPlayer pronunciationRecordPlayer;

		private TranslateTextExercise exercise;

		public TranslateTextExercise Exercise
		{
			get => exercise;
			private set
			{
				SetProperty(ref exercise, value);
				OnPropertyChanged(nameof(DisplayedTextInKnownLanguage));
				OnPropertyChanged(nameof(HintForOtherSynonyms));
			}
		}

		public string DisplayedTextInKnownLanguage => Exercise.GetTranslationsInKnownLanguage();

		public string HintForOtherSynonyms => Exercise.GetHintForOtherSynonyms();

		private PronunciationRecord currentPronunciationRecord;

		private PronunciationRecord CurrentPronunciationRecord
		{
			get => currentPronunciationRecord;
			set
			{
				currentPronunciationRecord = value;
				OnPropertyChanged(nameof(PronunciationRecordExists));
			}
		}

		public bool PronunciationRecordExists => CurrentPronunciationRecord != null;

		private bool typedTextIsFocused;

		public bool TypedTextIsFocused
		{
			get => typedTextIsFocused;
			set => SetProperty(ref typedTextIsFocused, value);
		}

		private string typedText;

		public string TypedText
		{
			get => typedText;
			set => SetProperty(ref typedText, value);
		}

		// We use a pair of properties - ExerciseWasPerformedCorrectly and ExerciseWasPerformedIncorrectly, because they are no actually inverted.
		// When exercise was not yet checked, both properties are set to false.
		// This state could be expressed as null value of bool? property, however this requires custom visibility value converter which converts null value to collapsed result.
		private bool exerciseWasPerformedCorrectly;

		public bool ExerciseWasPerformedCorrectly
		{
			get => exerciseWasPerformedCorrectly;
			private set => SetProperty(ref exerciseWasPerformedCorrectly, value);
		}

		private bool exerciseWasPerformedIncorrectly;

		public bool ExerciseWasPerformedIncorrectly
		{
			get => exerciseWasPerformedIncorrectly;
			private set => SetProperty(ref exerciseWasPerformedIncorrectly, value);
		}

		public ICommand NextStepCommand { get; }

		public ICommand PlayPronunciationRecordCommand { get; }

		public TranslateTextExerciseViewModel(ITutorService tutorService, IExerciseResultService exerciseResultService,
			IPronunciationRecordPlayer pronunciationRecordPlayer, IMessenger messenger, ISystemClock systemClock)
			: base(exerciseResultService, systemClock)
		{
			this.tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));
			this.pronunciationRecordPlayer = pronunciationRecordPlayer ?? throw new ArgumentNullException(nameof(pronunciationRecordPlayer));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			NextStepCommand = new RelayCommand(() => messenger.Send(new CheckOrSwitchToNextExerciseEventArgs()));
			PlayPronunciationRecordCommand = new AsyncRelayCommand(PlayPronunciationRecord);
		}

		public void Load(User user, TranslateTextExercise exercise)
		{
			Load(user);

			Exercise = exercise ?? throw new ArgumentNullException(nameof(exercise));

			SetFocus(() => TypedTextIsFocused);

			TypedText = String.Empty;

			ExerciseWasPerformedCorrectly = false;
			ExerciseWasPerformedIncorrectly = false;

			CurrentPronunciationRecord = null;
		}

		protected override async Task<BasicExerciseResult> CheckExerciseAndStoreResult(CancellationToken cancellationToken)
		{
			var exerciseResult = Exercise.Check(TypedText, CurrentTimestamp);

			ExerciseWasPerformedCorrectly = exerciseResult.IsSuccessful;
			ExerciseWasPerformedIncorrectly = exerciseResult.IsFailed;

			await ExerciseResultService.AddTranslateTextExerciseResult(User, Exercise, exerciseResult, cancellationToken);

			await PlayPronunciationRecord(cancellationToken);

			return exerciseResult;
		}

		private async Task PlayPronunciationRecord(CancellationToken cancellationToken)
		{
			CurrentPronunciationRecord = await tutorService.GetPronunciationRecord(Exercise.TextInStudiedLanguage.Id, cancellationToken);

			if (CurrentPronunciationRecord != null)
			{
				await pronunciationRecordPlayer.PlayPronunciationRecord(CurrentPronunciationRecord, cancellationToken);
			}
		}
	}
}
