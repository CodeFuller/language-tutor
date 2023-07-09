using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.Services.Interfaces.Repositories;
using VocabularyCoach.Services.LanguageTraits;

namespace VocabularyCoach.Services
{
	internal sealed class EditVocabularyService : IEditVocabularyService
	{
		private readonly ILanguageTextRepository languageTextRepository;

		private readonly IPronunciationRecordRepository pronunciationRecordRepository;

		private readonly IReadOnlyDictionary<ItemId, ILanguageTraits> languagesTraits;

		public EditVocabularyService(ILanguageTextRepository languageTextRepository, IPronunciationRecordRepository pronunciationRecordRepository, IEnumerable<ILanguageTraits> languagesTraits)
		{
			this.languageTextRepository = languageTextRepository ?? throw new ArgumentNullException(nameof(languageTextRepository));
			this.pronunciationRecordRepository = pronunciationRecordRepository ?? throw new ArgumentNullException(nameof(pronunciationRecordRepository));
			this.languagesTraits = languagesTraits?.ToDictionary(x => x.Language.Id, x => x) ?? throw new ArgumentNullException(nameof(languagesTraits));
		}

		public Task<IReadOnlyCollection<LanguageText>> GetLanguageTexts(Language language, CancellationToken cancellationToken)
		{
			return languageTextRepository.GetLanguageTexts(language.Id, cancellationToken);
		}

		public async Task<Uri> GetUrlForLanguageTextCheck(LanguageText languageText, CancellationToken cancellationToken)
		{
			if (!languagesTraits.TryGetValue(languageText.Language.Id, out var languageTraits))
			{
				throw new NotSupportedException($"Language is not supported: {languageText.Language}");
			}

			return await languageTraits.GetUrlForTextCheck(languageText.Text, cancellationToken);
		}

		public async Task<LanguageText> AddLanguageTextWithTranslation(LanguageTextCreationData languageTextData1, LanguageTextCreationData languageTextData2, CancellationToken cancellationToken)
		{
			var languageText1 = await AddLanguageText(languageTextData1, cancellationToken);
			var languageText2 = await AddLanguageText(languageTextData2, cancellationToken);

			await languageTextRepository.AddTranslation(languageText1, languageText2, cancellationToken);

			return languageText1;
		}

		private async Task<LanguageText> AddLanguageText(LanguageTextCreationData languageTextData, CancellationToken cancellationToken)
		{
			var languageText = new LanguageText
			{
				Language = languageTextData.Language,
				Text = languageTextData.Text,
				Note = languageTextData.Note,
			};

			await languageTextRepository.AddLanguageText(languageText, cancellationToken);

			if (languageTextData.PronunciationRecord != null)
			{
				await pronunciationRecordRepository.AddPronunciationRecord(languageText.Id, languageTextData.PronunciationRecord, cancellationToken);
			}

			return languageText;
		}
	}
}
