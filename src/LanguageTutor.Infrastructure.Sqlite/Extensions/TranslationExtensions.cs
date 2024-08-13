using System;
using LanguageTutor.Infrastructure.Sqlite.Entities;
using LanguageTutor.Models;

namespace LanguageTutor.Infrastructure.Sqlite.Extensions
{
	internal static class TranslationExtensions
	{
		public static Translation ToModel(this TranslationEntity translationEntity, int studiedLanguageId, int knownLanguageId)
		{
			return new Translation
			{
				Text1 = translationEntity.GetTextInLanguage(studiedLanguageId).ToModel(),
				Text2 = translationEntity.GetTextInLanguage(knownLanguageId).ToModel(),
			};
		}

		public static TextEntity GetTextInLanguage(this TranslationEntity translationEntity, int languageId)
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
