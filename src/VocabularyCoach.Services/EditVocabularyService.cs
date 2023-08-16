using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.Services.Interfaces.Repositories;
using VocabularyCoach.Services.Internal;

namespace VocabularyCoach.Services
{
	internal sealed class EditVocabularyService : IEditVocabularyService
	{
		private readonly ILanguageTextRepository languageTextRepository;

		private readonly IPronunciationRecordRepository pronunciationRecordRepository;

		private readonly ISystemClock systemClock;

		public EditVocabularyService(ILanguageTextRepository languageTextRepository, IPronunciationRecordRepository pronunciationRecordRepository, ISystemClock systemClock)
		{
			this.languageTextRepository = languageTextRepository ?? throw new ArgumentNullException(nameof(languageTextRepository));
			this.pronunciationRecordRepository = pronunciationRecordRepository ?? throw new ArgumentNullException(nameof(pronunciationRecordRepository));
			this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		}

		public Task<IReadOnlyCollection<LanguageText>> GetLanguageTexts(Language language, CancellationToken cancellationToken)
		{
			return languageTextRepository.GetLanguageTexts(language.Id, cancellationToken);
		}

		public Task<IReadOnlyCollection<Translation>> GetTranslations(Language language1, Language language2, CancellationToken cancellationToken)
		{
			return languageTextRepository.GetTranslations(language1.Id, language2.Id, cancellationToken);
		}

		public async Task<LanguageText> AddLanguageText(LanguageTextData languageTextData, CancellationToken cancellationToken)
		{
			var languageText = CreateLanguageText(null, languageTextData);

			await languageTextRepository.AddLanguageText(languageText, systemClock.Now, cancellationToken);

			if (languageTextData.PronunciationRecord != null)
			{
				await pronunciationRecordRepository.AddPronunciationRecord(languageText.Id, languageTextData.PronunciationRecord, cancellationToken);
			}

			return languageText;
		}

		private static LanguageText CreateLanguageText(ItemId id, LanguageTextData languageTextData)
		{
			return new LanguageText
			{
				Id = id,
				Language = languageTextData.Language,
				Text = languageTextData.Text,
				Note = String.IsNullOrWhiteSpace(languageTextData.Note) ? null : languageTextData.Note,
			};
		}

		public async Task<Translation> AddTranslation(LanguageText languageText1, LanguageText languageText2, CancellationToken cancellationToken)
		{
			var translation = new Translation
			{
				Text1 = languageText1,
				Text2 = languageText2,
			};

			await languageTextRepository.AddTranslation(translation, cancellationToken);

			return translation;
		}

		public async Task<LanguageText> UpdateLanguageText(LanguageText languageText, LanguageTextData newLanguageTextData, CancellationToken cancellationToken)
		{
			var newLanguageText = CreateLanguageText(languageText.Id, newLanguageTextData);

			await languageTextRepository.UpdateLanguageText(newLanguageText, cancellationToken);

			// Currently we support only update of pronunciation record.
			// We do not support adding or deletion of pronunciation record.
			if (newLanguageTextData.PronunciationRecord != null)
			{
				await pronunciationRecordRepository.UpdatePronunciationRecord(languageText.Id, newLanguageTextData.PronunciationRecord, cancellationToken);
			}

			return newLanguageText;
		}

		public Task DeleteLanguageText(LanguageText languageText, CancellationToken cancellationToken)
		{
			return languageTextRepository.DeleteLanguageText(languageText, cancellationToken);
		}

		public Task DeleteTranslation(Translation translation, CancellationToken cancellationToken)
		{
			return languageTextRepository.DeleteTranslation(translation, cancellationToken);
		}
	}
}
