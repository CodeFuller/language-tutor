using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Models.Extensions;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.Services.Interfaces.Repositories;
using LanguageTutor.Services.Internal;
using LanguageTutor.Services.Settings;
using Microsoft.Extensions.Options;

namespace LanguageTutor.Services
{
	internal sealed class TutorService : ITutorService
	{
		private readonly ILanguageRepository languageRepository;

		private readonly IPronunciationRecordRepository pronunciationRecordRepository;

		private readonly IExerciseRepository exerciseRepository;

		private readonly IStatisticsRepository statisticsRepository;

		private readonly IExerciseFactory exerciseFactory;

		private readonly IExercisesSelector exercisesSelector;

		private readonly IProblematicExercisesProvider problematicExercisesProvider;

		private readonly ISystemClock systemClock;

		private readonly ExercisesSettings settings;

		private DateOnly Today => systemClock.Today;

		public TutorService(ILanguageRepository languageRepository, IPronunciationRecordRepository pronunciationRecordRepository, IExerciseRepository exerciseRepository,
			IStatisticsRepository statisticsRepository, IExerciseFactory exerciseFactory, IExercisesSelector exercisesSelector,
			IProblematicExercisesProvider problematicExercisesProvider, ISystemClock systemClock, IOptions<ExercisesSettings> options)
		{
			this.languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
			this.pronunciationRecordRepository = pronunciationRecordRepository ?? throw new ArgumentNullException(nameof(pronunciationRecordRepository));
			this.exerciseRepository = exerciseRepository ?? throw new ArgumentNullException(nameof(exerciseRepository));
			this.statisticsRepository = statisticsRepository ?? throw new ArgumentNullException(nameof(statisticsRepository));
			this.exerciseFactory = exerciseFactory ?? throw new ArgumentNullException(nameof(exerciseFactory));
			this.exercisesSelector = exercisesSelector ?? throw new ArgumentNullException(nameof(exercisesSelector));
			this.problematicExercisesProvider = problematicExercisesProvider ?? throw new ArgumentNullException(nameof(problematicExercisesProvider));
			this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken)
		{
			var languages = await languageRepository.GetLanguages(cancellationToken);

			return languages.OrderBy(x => x.Name).ToList();
		}

		public async Task<IReadOnlyCollection<BasicExercise>> GetExercisesToPerform(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var exercises = await GetExercises(user, studiedLanguage, knownLanguage, cancellationToken);

			return exercisesSelector.SelectExercisesToPerform(Today, exercises, settings.DailyLimit);
		}

		public async Task<IReadOnlyCollection<BasicExercise>> GetProblematicExercises(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var exercises = await GetExercises(user, studiedLanguage, knownLanguage, cancellationToken);

			return problematicExercisesProvider.GetProblematicExercises(exercises);
		}

		private async Task<IEnumerable<BasicExercise>> GetExercises(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var translateTextExercises = await GetTranslateTextExercises(user.Id, studiedLanguage.Id, knownLanguage.Id, cancellationToken);

			var inflectWordExercises = await GetInflectWordExercises(user.Id, studiedLanguage.Id, cancellationToken);

			return translateTextExercises.Concat<BasicExercise>(inflectWordExercises);
		}

		private async Task<IEnumerable<TranslateTextExercise>> GetTranslateTextExercises(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, CancellationToken cancellationToken)
		{
			var exercisesData = await exerciseRepository.GetTranslateTextExercises(userId, studiedLanguageId, knownLanguageId, cancellationToken);

			return exerciseFactory.CreateTranslateTextExercises(exercisesData);
		}

		private async Task<IEnumerable<InflectWordExercise>> GetInflectWordExercises(ItemId userId, ItemId studiedLanguageId, CancellationToken cancellationToken)
		{
			var exercisesData = await exerciseRepository.GetInflectWordExercises(userId, studiedLanguageId, cancellationToken);

			return exerciseFactory.CreateInflectWordExercises(exercisesData);
		}

		public Task<PronunciationRecord> GetPronunciationRecord(ItemId textId, CancellationToken cancellationToken)
		{
			return pronunciationRecordRepository.GetPronunciationRecord(textId, cancellationToken);
		}

		public Task<UserStatisticsData> GetTodayUserStatistics(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			return GetUserStatisticsForDate(user, studiedLanguage, knownLanguage, Today, cancellationToken);
		}

		private async Task<UserStatisticsData> GetUserStatisticsForDate(User user, Language studiedLanguage, Language knownLanguage, DateOnly date, CancellationToken cancellationToken)
		{
			var exercises = (await GetExercises(user, studiedLanguage, knownLanguage, cancellationToken))
				.Where(x => x.CreationTimestamp.ToDateOnly() <= date)
				.Select(x => x.WithLimitedResults(date))
				.ToList();

			var exercisesToPerform = exercisesSelector.SelectExercisesToPerform(date, exercises, settings.DailyLimit);
			var exercisesToPerformIfNoLimit = exercisesSelector.SelectExercisesToPerform(date, exercises, Int32.MaxValue);
			var problematicExercises = problematicExercisesProvider.GetProblematicExercises(exercises);

			var previousDate = date.AddDays(-1);
			var totalNumberOfExercisesLearnedForDate = exercises.Count(x => ExerciseIsLearned(x, date));
			var totalNumberOfExercisesLearnedForPreviousDate = exercises.Count(x => ExerciseIsLearned(x, previousDate));

			return new UserStatisticsData
			{
				Date = date,
				TotalNumberOfExercises = exercises.Count,
				TotalNumberOfLearnedExercises = totalNumberOfExercisesLearnedForDate,
				NumberOfProblematicExercises = problematicExercises.Count,
				RestNumberOfExercisesToPerformToday = exercisesToPerform.Count,
				RestNumberOfExercisesToPerformTodayIfNoLimit = exercisesToPerformIfNoLimit.Count,
				NumberOfExercisesPerformedToday = exercises.Count(x => x.SortedResults.Any(y => y.DateTime.ToDateOnly() == date)),
				NumberOfExercisesLearnedToday = totalNumberOfExercisesLearnedForDate - totalNumberOfExercisesLearnedForPreviousDate,
			};
		}

		public async Task UpdateTodayUserStatistics(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			// Filling also missing statistics for previous dates if any.
			var existingUserStatistics = (await statisticsRepository.GetUserStatistics(user.Id, studiedLanguage.Id, knownLanguage.Id, cancellationToken))
				.OrderBy(x => x.Date)
				.ToList();

			var today = Today;

			var firstDateForUpdate = existingUserStatistics.Any() && existingUserStatistics.Last().Date < today ? existingUserStatistics.Last().Date.AddDays(1) : today;

			for (var date = firstDateForUpdate; date <= today; date = date.AddDays(1))
			{
				var userStatisticsForDate = await GetUserStatisticsForDate(user, studiedLanguage, knownLanguage, date, cancellationToken);
				await statisticsRepository.UpdateUserStatistics(user.Id, studiedLanguage.Id, knownLanguage.Id, userStatisticsForDate, cancellationToken);
			}
		}

		public async Task<IReadOnlyCollection<UserStatisticsData>> GetUserStatisticsHistory(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var statistics = await statisticsRepository.GetUserStatistics(user.Id, studiedLanguage.Id, knownLanguage.Id, cancellationToken);

			return statistics.OrderBy(x => x.Date).ToList();
		}

		private static bool ExerciseIsLearned(BasicExercise exercise, DateOnly date)
		{
			// We consider exercise as learned, if 3 last results are successful.
			const int learnedExerciseResultsNumber = 3;

			var lastResults = exercise.WithLimitedResults(date, learnedExerciseResultsNumber).SortedResults;

			return lastResults.Count >= learnedExerciseResultsNumber && lastResults.All(x => x.IsSuccessful);
		}
	}
}
