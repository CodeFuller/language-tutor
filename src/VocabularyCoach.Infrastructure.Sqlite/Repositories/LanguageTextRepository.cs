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

		public async Task<IReadOnlyCollection<LanguageText>> GetLanguageText(ItemId languageId, CancellationToken cancellationToken)
		{
			await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

			var texts = await context.Texts
				.Include(x => x.Language)
				.Where(x => x.LanguageId == languageId.ToInt32())
				.ToListAsync(cancellationToken);

			return texts.Select(x => x.ToModel()).ToList();
		}

		public async Task<IReadOnlyCollection<StudiedTextWithTranslation>> GetStudiedTexts(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, CancellationToken cancellationToken)
		{
			await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

			var translations = from text1 in context.Texts.Include(x => x.Language)
							   where text1.LanguageId == studiedLanguageId.ToInt32()
							   join translation in context.Translations on text1.Id equals translation.TextId1
							   join text2 in context.Texts.Include(x => x.Language).Where(x => x.LanguageId == knownLanguageId.ToInt32()) on translation.TextId2 equals text2.Id
							   select new
							   {
								   TextInStudiedLanguage = text1,
								   TextInKnownLanguage = text2,
							   };

			var studiedTextsWithCheckResults = await (from textPair in translations
													  join checkResult in context.CheckResults.Where(x => x.UserId == userId.ToInt32()) on textPair.TextInStudiedLanguage.Id equals checkResult.TextId into grouping
													  from checkResult in grouping.DefaultIfEmpty()
													  select new
													  {
														  textPair.TextInStudiedLanguage,
														  textPair.TextInKnownLanguage,
														  CheckResult = checkResult,
													  })
				.ToListAsync(cancellationToken);

			return studiedTextsWithCheckResults
				.GroupBy(x => x.TextInStudiedLanguage.Id)
				.Select(x => CreateStudiedTextWithTranslation(x.First().TextInStudiedLanguage, x.First().TextInKnownLanguage, x.Select(y => y.CheckResult).ToList()))
				.ToList();
		}

		public async Task AddLanguageText(LanguageText languageText, CancellationToken cancellationToken)
		{
			var textEntity = languageText.ToEntity();

			await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

			await context.Texts.AddAsync(textEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			languageText.Id = textEntity.Id.ToItemId();
		}

		public async Task AddTranslation(LanguageText languageText1, LanguageText languageText2, CancellationToken cancellationToken)
		{
			await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

			var translationEntity = new TranslationEntity
			{
				TextId1 = languageText1.Id.ToInt32(),
				TextId2 = languageText2.Id.ToInt32(),
			};

			await context.Translations.AddAsync(translationEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);
		}

		private static StudiedTextWithTranslation CreateStudiedTextWithTranslation(TextEntity textInStudiedLanguage, TextEntity textInKnownLanguage, IReadOnlyCollection<CheckResultEntity> checkResults)
		{
			// https://learn.microsoft.com/en-us/ef/core/querying/complex-query-operators#left-join
			if (checkResults.Count == 1 && checkResults.Single() == null)
			{
				checkResults = new List<CheckResultEntity>();
			}

			var studiedText = new StudiedText
			{
				LanguageText = textInStudiedLanguage.ToModel(),
			};

			foreach (var checkResult in checkResults)
			{
				studiedText.AddCheckResult(checkResult.ToModel());
			}

			return new StudiedTextWithTranslation
			{
				StudiedText = studiedText,
				TextInKnownLanguage = textInKnownLanguage.ToModel(),
			};
		}
	}
}