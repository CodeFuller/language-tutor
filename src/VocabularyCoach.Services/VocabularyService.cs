using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
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

		private readonly ISystemClock systemClock;

		private DateOnly Today => systemClock.Today;

		public VocabularyService(ILanguageRepository languageRepository, ILanguageTextRepository languageTextRepository,
			IPronunciationRecordRepository pronunciationRecordRepository, ICheckResultRepository checkResultRepository,
			IStatisticsRepository statisticsRepository, ISynonymGrouper synonymGrouper, ITextsForPracticeSelector textsForPracticeSelector, ISystemClock systemClock)
		{
			this.languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
			this.languageTextRepository = languageTextRepository ?? throw new ArgumentNullException(nameof(languageTextRepository));
			this.pronunciationRecordRepository = pronunciationRecordRepository ?? throw new ArgumentNullException(nameof(pronunciationRecordRepository));
			this.checkResultRepository = checkResultRepository ?? throw new ArgumentNullException(nameof(checkResultRepository));
			this.statisticsRepository = statisticsRepository ?? throw new ArgumentNullException(nameof(statisticsRepository));
			this.synonymGrouper = synonymGrouper ?? throw new ArgumentNullException(nameof(synonymGrouper));
			this.textsForPracticeSelector = textsForPracticeSelector ?? throw new ArgumentNullException(nameof(textsForPracticeSelector));
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

			return textsForPracticeSelector.SelectTextsForTodayPractice(studiedTexts);
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
			var checkResult = new CheckResult
			{
				DateTime = systemClock.Now,
				CheckResultType = GetCheckResultType(studiedText.TextInStudiedLanguage, typedText),
			};

			await checkResultRepository.AddCheckResult(user.Id, studiedText.TextInStudiedLanguage.Id, checkResult, cancellationToken);

			studiedText.AddCheckResult(checkResult);

			return checkResult.CheckResultType;
		}

		public async Task<UserStatisticsData> GetUserStatistics(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var studiedTexts = (await GetStudiedTexts(user, studiedLanguage, knownLanguage, cancellationToken)).ToList();

			var textsForPractice = textsForPracticeSelector.SelectTextsForTodayPractice(studiedTexts);

			return new UserStatisticsData
			{
				TotalNumberOfTexts = studiedTexts.Count,
				TotalNumberOfLearnedTexts = studiedTexts.Count(TextIsLearned),
				NumberOfTextsPracticedToday = studiedTexts.Count(text => text.CheckResults.Any(checkResult => DateOnly.FromDateTime(checkResult.DateTime.Date) == Today)),
				RestNumberOfTextsToPracticeToday = textsForPractice.Count,
			};
		}

		public async Task StoreUserStatistics(User user, Language studiedLanguage, Language knownLanguage, UserStatisticsData statistics, CancellationToken cancellationToken)
		{
			await statisticsRepository.UpdateUserStatistics(user.Id, studiedLanguage.Id, knownLanguage.Id, Today, statistics, cancellationToken);
		}

		private static bool TextIsLearned(StudiedText studiedText)
		{
			// We consider text as learned, if 3 last checks are successful.
			const int learnedTextChecksNumber = 3;

			var lastChecks = studiedText.CheckResults
				.Take(learnedTextChecksNumber)
				.ToList();

			return lastChecks.Count >= 3 && lastChecks.All(x => x.CheckResultType == CheckResultType.Ok);
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
