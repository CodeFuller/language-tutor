using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Infrastructure.Sqlite.Entities;
using LanguageTutor.Infrastructure.Sqlite.Extensions;
using LanguageTutor.Infrastructure.Sqlite.Internal;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LanguageTutor.Infrastructure.Sqlite.Repositories
{
	internal sealed class ExerciseRepository : IExerciseRepository
	{
		private readonly IDbContextFactory<LanguageTutorDbContext> contextFactory;

		public ExerciseRepository(IDbContextFactory<LanguageTutorDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task<IReadOnlyCollection<TranslateTextExerciseData>> GetTranslateTextExercises(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var studiedLanguageDbId = studiedLanguageId.ToInt32();
			var knownLanguageDbId = knownLanguageId.ToInt32();

			var translations1 = await dbContext.GetTranslationsQueryable(studiedLanguageDbId, knownLanguageDbId)
				.Include(x => x.Text1)
				.ToListAsync(cancellationToken);

			var translations2 = await dbContext.GetTranslationsQueryable(knownLanguageDbId, studiedLanguageDbId)
				.Include(x => x.Text2)
				.ToListAsync(cancellationToken);

			// We used to load TranslateTextExerciseResults as JOIN with Texts, i.e. dbContext.Translations.Include(x => x.Text1).ThenInclude(x => x.TranslateTextExerciseResults.Where(y => y.UserId == userId.ToInt32())).
			// However this approach works very slow.
			// That is why we replaced it with a trick: TranslateTextExerciseResults are loaded with a separate query.
			// This load will populate TranslateTextExerciseResults property in TextEntity unless it has no exercise results yet.
			await dbContext.TranslateTextExerciseResults.Where(x => x.UserId == userId.ToInt32()).ToListAsync(cancellationToken);

			return translations1.Concat(translations2)
				.Select(x => new
				{
					TextInStudiedLanguage = x.GetTextInLanguage(studiedLanguageDbId),
					TextInKnownLanguage = x.GetTextInLanguage(knownLanguageDbId),
				})
				.GroupBy(x => x.TextInStudiedLanguage)
				.Select(x => new TranslateTextExerciseData
				{
					TextInStudiedLanguage = x.Key.ToModel(),
					TextsInKnownLanguage = x.Select(y => y.TextInKnownLanguage.ToModel()).ToList(),
					ExerciseResults = (x.Key.TranslateTextExerciseResults?.Select(ToTranslateTextExerciseResult) ?? []).ToList(),
				})
				.ToList();
		}

		public Task<IReadOnlyCollection<InflectWordExerciseData>> GetInflectWordExercises(ItemId userId, ItemId studiedLanguageId, CancellationToken cancellationToken)
		{
			// TODO: Implement DAL for InflectWordExercise.
			throw new NotImplementedException();
		}

		private static TranslateTextExerciseResult ToTranslateTextExerciseResult(TranslateTextExerciseResultEntity entity)
		{
			return new(entity.DateTime, entity.ResultType, entity.TypedText)
			{
				Id = entity.Id.ToItemId(),
			};
		}
	}
}
