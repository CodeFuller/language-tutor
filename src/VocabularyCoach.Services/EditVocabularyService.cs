using System;
using System.Collections.Generic;
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

		private readonly ISupportedLanguageTraits supportedLanguageTraits;

		public EditVocabularyService(ILanguageTextRepository languageTextRepository, IPronunciationRecordRepository pronunciationRecordRepository, ISupportedLanguageTraits supportedLanguageTraits)
		{
			this.languageTextRepository = languageTextRepository ?? throw new ArgumentNullException(nameof(languageTextRepository));
			this.pronunciationRecordRepository = pronunciationRecordRepository ?? throw new ArgumentNullException(nameof(pronunciationRecordRepository));
			this.supportedLanguageTraits = supportedLanguageTraits ?? throw new ArgumentNullException(nameof(supportedLanguageTraits));
		}

		public Task<IReadOnlyCollection<LanguageText>> GetLanguageTexts(Language language, CancellationToken cancellationToken)
		{
			return languageTextRepository.GetLanguageTexts(language.Id, cancellationToken);
		}

		public Task<Uri> GetUrlForLanguageTextCheck(LanguageText languageText, CancellationToken cancellationToken)
		{
			var url = supportedLanguageTraits.GetLanguageTraits(languageText.Language)
				.GetUrlForTextCheck(languageText.Text);

			return Task.FromResult(url);
		}

		public async Task<LanguageText> AddLanguageTextWithTranslation(LanguageTextCreationData languageTextData1, LanguageTextCreationData languageTextData2, CancellationToken cancellationToken)
		{
			if (languageTextData1.PronunciationRecord == null)
			{
				throw new InvalidOperationException("Pronunciation record is not set");
			}

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
				Note = String.IsNullOrWhiteSpace(languageTextData.Note) ? null : languageTextData.Note,
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
