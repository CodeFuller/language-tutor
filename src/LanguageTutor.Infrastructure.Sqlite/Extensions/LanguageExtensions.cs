using LanguageTutor.Infrastructure.Sqlite.Entities;
using LanguageTutor.Models;

namespace LanguageTutor.Infrastructure.Sqlite.Extensions
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
