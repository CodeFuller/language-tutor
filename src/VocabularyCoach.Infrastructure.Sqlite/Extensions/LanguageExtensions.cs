using VocabularyCoach.Infrastructure.Sqlite.Entities;
using VocabularyCoach.Models;

namespace VocabularyCoach.Infrastructure.Sqlite.Extensions
{
	internal static class LanguageExtensions
	{
		public static Language ToModel(this LanguageEntity languageEntity)
		{
			return new Language
			{
				Id = languageEntity.Id.ToItemId(),
				Name = languageEntity.Name,
			};
		}
	}
}
