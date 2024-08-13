using System.Linq;
using LanguageTutor.Infrastructure.Sqlite.Entities;
using LanguageTutor.Infrastructure.Sqlite.Internal;
using Microsoft.EntityFrameworkCore;

namespace LanguageTutor.Infrastructure.Sqlite.Extensions
{
	internal static class LanguageTutorDbContextExtensions
	{
		public static IQueryable<TranslationEntity> GetTranslationsQueryable(this LanguageTutorDbContext dbContext, int language1Id, int language2Id)
		{
			return dbContext.Translations
				.Include(x => x.Text1).ThenInclude(x => x.Language)
				.Include(x => x.Text2).ThenInclude(x => x.Language)
				.Where(x => x.Text1.LanguageId == language1Id)
				.Where(x => x.Text2.LanguageId == language2Id);
		}
	}
}
