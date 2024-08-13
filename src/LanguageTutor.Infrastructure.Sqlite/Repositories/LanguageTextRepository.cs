using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Infrastructure.Sqlite.Extensions;
using LanguageTutor.Infrastructure.Sqlite.Internal;
using LanguageTutor.Models;
using LanguageTutor.Services.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LanguageTutor.Infrastructure.Sqlite.Repositories
{
	internal sealed class LanguageTextRepository : ILanguageTextRepository
	{
		private readonly IDbContextFactory<LanguageTutorDbContext> contextFactory;

		public LanguageTextRepository(IDbContextFactory<LanguageTutorDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task<IReadOnlyCollection<LanguageText>> GetLanguageTexts(ItemId languageId, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var texts = await dbContext.Texts
				.Include(x => x.Language)
				.Where(x => x.LanguageId == languageId.ToInt32())
				.ToListAsync(cancellationToken);

			return texts.Select(x => x.ToModel()).ToList();
		}

		public async Task<IReadOnlyCollection<Translation>> GetTranslations(ItemId language1Id, ItemId language2Id, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var language1DbId = language1Id.ToInt32();
			var language2DbId = language2Id.ToInt32();

			var translations = await dbContext.GetTranslationsQueryable(language1DbId, language2DbId)
				.Concat(dbContext.GetTranslationsQueryable(language2DbId, language1DbId))
				.ToListAsync(cancellationToken);

			return translations
				.Select(x => x.ToModel(language1DbId, language2DbId))
				.ToList();
		}

		public async Task AddLanguageText(LanguageText languageText, CancellationToken cancellationToken)
		{
			var textEntity = languageText.ToEntity();

			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			await dbContext.Texts.AddAsync(textEntity, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);

			languageText.Id = textEntity.Id.ToItemId();
		}

		public async Task AddTranslation(Translation translation, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var translationEntity = translation.ToEntity();

			await dbContext.Translations.AddAsync(translationEntity, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task UpdateLanguageText(LanguageText languageText, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var textEntity = await dbContext.Texts.SingleAsync(x => x.Id == languageText.Id.ToInt32(), cancellationToken);

			textEntity.Text = languageText.Text;
			textEntity.Note = languageText.Note;
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task DeleteLanguageText(LanguageText languageText, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var textEntity = await dbContext.Texts.SingleAsync(x => x.Id == languageText.Id.ToInt32(), cancellationToken);

			dbContext.Texts.Remove(textEntity);
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task DeleteTranslation(Translation translation, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var translationEntity = await dbContext.Translations.SingleAsync(x => x.Text1Id == translation.Text1.Id.ToInt32() && x.Text2Id == translation.Text2.Id.ToInt32(), cancellationToken);

			dbContext.Translations.Remove(translationEntity);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
