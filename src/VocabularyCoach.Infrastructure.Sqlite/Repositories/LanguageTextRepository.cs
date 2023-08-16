using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VocabularyCoach.Infrastructure.Sqlite.Entities;
using VocabularyCoach.Infrastructure.Sqlite.Extensions;
using VocabularyCoach.Infrastructure.Sqlite.Internal;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces.Repositories;

namespace VocabularyCoach.Infrastructure.Sqlite.Repositories
{
	internal sealed class LanguageTextRepository : ILanguageTextRepository
	{
		private readonly IDbContextFactory<VocabularyCoachDbContext> contextFactory;

		public LanguageTextRepository(IDbContextFactory<VocabularyCoachDbContext> contextFactory)
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

			var translations = await GetTranslationsQueryable(dbContext, language1Id, language2Id).ToListAsync(cancellationToken);

			return translations.Select(x => x.ToModel()).ToList();
		}

		public async Task<IReadOnlyCollection<StudiedTranslationData>> GetStudiedTranslations(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var translations = await GetTranslationsQueryable(dbContext, studiedLanguageId, knownLanguageId)
				.Include(x => x.Text1).ThenInclude(x => x.CheckResults.Where(y => y.UserId == userId.ToInt32()))
				.ToListAsync(cancellationToken);

			return translations
				.Select(x => new StudiedTranslationData
				{
					TextInStudiedLanguage = x.Text1.ToModel(),
					TextInKnownLanguage = x.Text2.ToModel(),
					CheckResults = x.Text1.CheckResults.Select(y => y.ToModel()).ToList(),
				})
				.ToList();
		}

		private static IQueryable<TranslationEntity> GetTranslationsQueryable(VocabularyCoachDbContext dbContext, ItemId language1Id, ItemId language2Id)
		{
			// TODO: Cover also translations from language2 to language1?
			return dbContext.Translations
				.Include(x => x.Text1).ThenInclude(x => x.Language)
				.Include(x => x.Text2).ThenInclude(x => x.Language)
				.Where(x => x.Text1.LanguageId == language1Id.ToInt32())
				.Where(x => x.Text2.LanguageId == language2Id.ToInt32());
		}

		public async Task AddLanguageText(LanguageText languageText, DateTimeOffset creationTimestamp, CancellationToken cancellationToken)
		{
			var textEntity = languageText.ToEntity(creationTimestamp);

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
