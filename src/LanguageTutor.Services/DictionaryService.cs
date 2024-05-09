using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.Services.Interfaces.Repositories;
using LanguageTutor.Services.Internal;

namespace LanguageTutor.Services
{
	internal sealed class DictionaryService : IDictionaryService
	{
		private readonly ILanguageTextRepository languageTextRepository;

		private readonly IPronunciationRecordRepository pronunciationRecordRepository;

		private readonly ISystemClock systemClock;

		public DictionaryService(ILanguageTextRepository languageTextRepository, IPronunciationRecordRepository pronunciationRecordRepository, ISystemClock systemClock)
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
			var languageText = new LanguageText
			{
				Language = languageTextData.Language,
				Text = languageTextData.Text,
				Note = NormalizeNote(languageTextData.Note),
				CreationTimestamp = systemClock.Now,
			};

			await languageTextRepository.AddLanguageText(languageText, cancellationToken);

			if (languageTextData.PronunciationRecord != null)
			{
				await pronunciationRecordRepository.AddPronunciationRecord(languageText.Id, languageTextData.PronunciationRecord, cancellationToken);
			}

			return languageText;
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
			var updatedLanguageText = new LanguageText
			{
				Id = languageText.Id,
				Language = newLanguageTextData.Language,
				Text = newLanguageTextData.Text,
				Note = NormalizeNote(newLanguageTextData.Note),
				CreationTimestamp = languageText.CreationTimestamp,
			};

			await languageTextRepository.UpdateLanguageText(updatedLanguageText, cancellationToken);

			// Currently we support only update of pronunciation record.
			// We do not support adding or deletion of pronunciation record.
			if (newLanguageTextData.PronunciationRecord != null)
			{
				await pronunciationRecordRepository.UpdatePronunciationRecord(languageText.Id, newLanguageTextData.PronunciationRecord, cancellationToken);
			}

			return updatedLanguageText;
		}

		private static string NormalizeNote(string note)
		{
			return String.IsNullOrWhiteSpace(note) ? null : note;
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
