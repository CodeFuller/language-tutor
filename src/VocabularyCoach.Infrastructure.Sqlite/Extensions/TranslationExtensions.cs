using VocabularyCoach.Infrastructure.Sqlite.Entities;
using VocabularyCoach.Models;

namespace VocabularyCoach.Infrastructure.Sqlite.Extensions
{
	internal static class TranslationExtensions
	{
		public static Translation ToModel(this TranslationEntity translationEntity)
		{
			return new Translation
			{
				Text1 = translationEntity.Text1.ToModel(),
				Text2 = translationEntity.Text2.ToModel(),
			};
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
