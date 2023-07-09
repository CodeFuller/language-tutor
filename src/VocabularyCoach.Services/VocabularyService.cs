using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.Services.Interfaces.Repositories;

namespace VocabularyCoach.Services
{
	internal sealed class VocabularyService : IVocabularyService
	{
		private readonly ILanguageRepository languageRepository;

		private readonly ILanguageTextRepository languageTextRepository;

		private readonly IPronunciationRecordRepository pronunciationRecordRepository;

		private readonly ICheckResultRepository checkResultRepository;

		public VocabularyService(ILanguageRepository languageRepository, ILanguageTextRepository languageTextRepository,
			IPronunciationRecordRepository pronunciationRecordRepository, ICheckResultRepository checkResultRepository)
		{
			this.languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
			this.languageTextRepository = languageTextRepository ?? throw new ArgumentNullException(nameof(languageTextRepository));
			this.pronunciationRecordRepository = pronunciationRecordRepository ?? throw new ArgumentNullException(nameof(pronunciationRecordRepository));
			this.checkResultRepository = checkResultRepository ?? throw new ArgumentNullException(nameof(checkResultRepository));
		}

		public async Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken)
		{
			var languages = await languageRepository.GetLanguages(cancellationToken);

			return languages.OrderBy(x => x.Name).ToList();
		}

		public Task<IReadOnlyCollection<StudiedTextWithTranslation>> GetStudiedTexts(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			return languageTextRepository.GetStudiedTexts(user.Id, studiedLanguage.Id, knownLanguage.Id, cancellationToken);
		}

		public Task<PronunciationRecord> GetPronunciationRecord(ItemId textId, CancellationToken cancellationToken)
		{
			return pronunciationRecordRepository.GetPronunciationRecord(textId, cancellationToken);
		}

		public async Task<CheckResultType> CheckTypedText(User user, StudiedText studiedText, string typedText, CancellationToken cancellationToken)
		{
			var checkResult = new CheckResult
			{
				DateTime = DateTimeOffset.Now,
				CheckResultType = GetCheckResultType(studiedText.LanguageText, typedText),
			};

			await checkResultRepository.AddCheckResult(user.Id, studiedText.LanguageText.Id, checkResult, cancellationToken);

			studiedText.AddCheckResult(checkResult);

			return checkResult.CheckResultType;
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
