using LanguageTutor.Infrastructure.Sqlite.Entities;
using LanguageTutor.Models;

namespace LanguageTutor.Infrastructure.Sqlite.Extensions
{
	internal static class TextExtensions
	{
		public static LanguageText ToModel(this TextEntity textEntity)
		{
			return new LanguageText
			{
				Id = textEntity.Id.ToItemId(),
				Language = textEntity.Language.ToModel(),
				Text = textEntity.Text,
				Note = textEntity.Note,
				CreationTimestamp = textEntity.CreationTimestamp,
			};
		}

		public static TextEntity ToEntity(this LanguageText model)
		{
			return new TextEntity
			{
				Id = model.Id?.ToInt32() ?? default,
				LanguageId = model.Language.Id.ToInt32(),
				Text = model.Text,
				Note = model.Note,
				CreationTimestamp = model.CreationTimestamp,
			};
		}
	}
}
