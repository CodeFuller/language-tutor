using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Extensions;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.Services.Interfaces.Repositories;
using VocabularyCoach.Services.Internal;

namespace VocabularyCoach.Services
{
	internal sealed class VocabularyService : IVocabularyService
	{
		private readonly ILanguageRepository languageRepository;

		private readonly ILanguageTextRepository languageTextRepository;

		private readonly IPronunciationRecordRepository pronunciationRecordRepository;

		private readonly ICheckResultRepository checkResultRepository;

		private readonly IStatisticsRepository statisticsRepository;

		private readonly ISynonymGrouper synonymGrouper;

		private readonly ITextsForPracticeSelector textsForPracticeSelector;

		private readonly IProblematicTextsSelector problematicTextsSelector;

		private readonly ISystemClock systemClock;

		private DateOnly Today => systemClock.Today;

		public VocabularyService(ILanguageRepository languageRepository, ILanguageTextRepository languageTextRepository,
			IPronunciationRecordRepository pronunciationRecordRepository, ICheckResultRepository checkResultRepository,
			IStatisticsRepository statisticsRepository, ISynonymGrouper synonymGrouper,
			ITextsForPracticeSelector textsForPracticeSelector, IProblematicTextsSelector problematicTextsSelector, ISystemClock systemClock)
		{
			this.languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
			this.languageTextRepository = languageTextRepository ?? throw new ArgumentNullException(nameof(languageTextRepository));
			this.pronunciationRecordRepository = pronunciationRecordRepository ?? throw new ArgumentNullException(nameof(pronunciationRecordRepository));
			this.checkResultRepository = checkResultRepository ?? throw new ArgumentNullException(nameof(checkResultRepository));
			this.statisticsRepository = statisticsRepository ?? throw new ArgumentNullException(nameof(statisticsRepository));
			this.synonymGrouper = synonymGrouper ?? throw new ArgumentNullException(nameof(synonymGrouper));
			this.textsForPracticeSelector = textsForPracticeSelector ?? throw new ArgumentNullException(nameof(textsForPracticeSelector));
			this.problematicTextsSelector = problematicTextsSelector ?? throw new ArgumentNullException(nameof(problematicTextsSelector));
			this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		}

		public async Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken)
		{
			var languages = await languageRepository.GetLanguages(cancellationToken);

			return languages.OrderBy(x => x.Name).ToList();
		}

		public async Task<IReadOnlyCollection<StudiedText>> GetTextsForPractice(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var studiedTexts = await GetStudiedTexts(user, studiedLanguage, knownLanguage, cancellationToken);

			return textsForPracticeSelector.GetTextsForPractice(Today, studiedTexts);
		}

		public async Task<IReadOnlyCollection<StudiedText>> GetProblematicTexts(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var studiedTexts = await GetStudiedTexts(user, studiedLanguage, knownLanguage, cancellationToken);

			return problematicTextsSelector.GetProblematicTexts(studiedTexts);
		}

		private async Task<IEnumerable<StudiedText>> GetStudiedTexts(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var studiedTranslations = await languageTextRepository.GetStudiedTranslations(user.Id, studiedLanguage.Id, knownLanguage.Id, cancellationToken);

			return synonymGrouper.GroupStudiedTranslationsBySynonyms(studiedTranslations);
		}

		public Task<PronunciationRecord> GetPronunciationRecord(ItemId textId, CancellationToken cancellationToken)
		{
			return pronunciationRecordRepository.GetPronunciationRecord(textId, cancellationToken);
		}

		public async Task<CheckResultType> CheckTypedText(User user, StudiedText studiedText, string typedText, CancellationToken cancellationToken)
		{
			var checkResultType = GetCheckResultType(studiedText.TextInStudiedLanguage, typedText);

			var checkResult = new CheckResult
			{
				DateTime = systemClock.Now,
				CheckResultType = checkResultType,
				TypedText = checkResultType == CheckResultType.Misspelled ? typedText : null,
			};

			await checkResultRepository.AddCheckResult(user.Id, studiedText.TextInStudiedLanguage.Id, checkResult, cancellationToken);

			studiedText.AddCheckResult(checkResult);

			return checkResult.CheckResultType;
		}

		public Task<UserStatisticsData> GetTodayUserStatistics(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			return GetUserStatisticsForDate(user, studiedLanguage, knownLanguage, Today, cancellationToken);
		}

		private async Task<UserStatisticsData> GetUserStatisticsForDate(User user, Language studiedLanguage, Language knownLanguage, DateOnly date, CancellationToken cancellationToken)
		{
			var studiedTexts = (await GetStudiedTexts(user, studiedLanguage, knownLanguage, cancellationToken))
				.Where(x => x.TextInStudiedLanguage.CreationTimestamp.ToDateOnly() <= date)
				.Select(x => x.WithLimitedCheckResults(date))
				.ToList();

			var textsForPractice = textsForPracticeSelector.GetTextsForPractice(date, studiedTexts);
			var problematicTexts = problematicTextsSelector.GetProblematicTexts(studiedTexts);

			var previousDate = date.AddDays(-1);
			var totalNumberOfTextsLearnedForToday = studiedTexts.Count(x => TextIsLearned(x, date));
			var totalNumberOfTextsLearnedForYesterday = studiedTexts.Count(x => TextIsLearned(x, previousDate));

			return new UserStatisticsData
			{
				Date = date,
				TotalNumberOfTexts = studiedTexts.Count,
				TotalNumberOfLearnedTexts = totalNumberOfTextsLearnedForToday,
				NumberOfProblematicTexts = problematicTexts.Count,
				RestNumberOfTextsToPracticeToday = textsForPractice.Count,
				NumberOfTextsPracticedToday = studiedTexts.Count(text => text.CheckResults.Any(checkResult => checkResult.DateTime.ToDateOnly() == date)),
				NumberOfTextsLearnedToday = totalNumberOfTextsLearnedForToday - totalNumberOfTextsLearnedForYesterday,
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

		private static bool TextIsLearned(StudiedText studiedText, DateOnly date)
		{
			// We consider text as learned, if 3 last checks are successful.
			const int learnedTextChecksNumber = 3;

			var lastChecks = studiedText.WithLimitedCheckResults(date, learnedTextChecksNumber).CheckResults;

			return lastChecks.Count >= learnedTextChecksNumber && lastChecks.All(x => x.IsSuccessful);
		}

		private static CheckResultType GetCheckResultType(LanguageText languageText, string typedText)
		{
			if (String.IsNullOrEmpty(typedText))
			{
				return CheckResultType.Skipped;
			}

			return String.Equals(languageText.Text, typedText, StringComparison.OrdinalIgnoreCase)
				? CheckResultType.Ok
				: CheckResultType.Misspelled;
		}
	}
}
