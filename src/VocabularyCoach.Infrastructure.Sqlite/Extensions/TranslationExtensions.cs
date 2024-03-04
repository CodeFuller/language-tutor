using System;
using System.Collections.Generic;
using System.Linq;
using VocabularyCoach.Infrastructure.Sqlite.Entities;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;

namespace VocabularyCoach.Infrastructure.Sqlite.Extensions
{
	internal static class TranslationExtensions
	{
		public static Translation ToModel(this TranslationEntity translationEntity, int studiedLanguageId, int knownLanguageId)
		{
			return new Translation
			{
				Text1 = GetTextInLanguage(translationEntity, studiedLanguageId).ToModel(),
				Text2 = GetTextInLanguage(translationEntity, knownLanguageId).ToModel(),
			};
		}

		public static StudiedTranslationData ToStudiedTranslationData(this TranslationEntity translationEntity, int studiedLanguageId, int knownLanguageId)
		{
			var textInStudiedLanguage = GetTextInLanguage(translationEntity, studiedLanguageId);

			return new StudiedTranslationData
			{
				TextInStudiedLanguage = textInStudiedLanguage.ToModel(),
				TextInKnownLanguage = GetTextInLanguage(translationEntity, knownLanguageId).ToModel(),
				CheckResults = textInStudiedLanguage.CheckResults?.Select(y => y.ToModel()).ToList() ?? new List<CheckResult>(),
			};
		}

		private static TextEntity GetTextInLanguage(TranslationEntity translationEntity, int languageId)
		{
			if (translationEntity.Text1.LanguageId == languageId)
			{
				return translationEntity.Text1;
			}

			if (translationEntity.Text2.LanguageId == languageId)
			{
				return translationEntity.Text2;
			}

			throw new InvalidOperationException($"Translation does not contain text with language {languageId}");
		}

		public static TranslationEntity ToEntity(this Translation model)
		{
			return new TranslationEntity
			{
				Text1Id = model.Text1.Id?.ToInt32() ?? default,
				Text2Id = model.Text2.Id?.ToInt32() ?? default,
			};
		}
	}
}
