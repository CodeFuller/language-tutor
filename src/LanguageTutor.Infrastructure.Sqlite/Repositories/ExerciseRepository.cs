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
using LanguageTutor.Models.Exercises.Inflection;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LanguageTutor.Infrastructure.Sqlite.Repositories
{
	internal sealed class ExerciseRepository : IExerciseRepository
	{
		private readonly IDbContextFactory<LanguageTutorDbContext> contextFactory;

		private readonly IJsonSerializer jsonSerializer;

		public ExerciseRepository(IDbContextFactory<LanguageTutorDbContext> contextFactory, IJsonSerializer jsonSerializer)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
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

		private static TranslateTextExerciseResult ToTranslateTextExerciseResult(TranslateTextExerciseResultEntity entity)
		{
			return new(entity.DateTime, entity.ResultType, entity.TypedText);
		}

		public async Task<IReadOnlyCollection<InflectWordExerciseData>> GetInflectWordExercises(ItemId userId, ItemId studiedLanguageId, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var exercises = await dbContext.InflectWordExercises.Where(x => x.LanguageId == studiedLanguageId.ToInt32()).ToListAsync(cancellationToken);
			var exerciseIds = exercises.Select(x => x.Id).ToHashSet();

			// See comment in method GetTranslateTextExercises().
			var resultEntities = await dbContext.InflectWordExerciseResults.Where(x => x.UserId == userId.ToInt32()).ToListAsync(cancellationToken);

			var results = resultEntities
				.Where(x => exerciseIds.Contains(x.ExerciseId))
				.ToLookup(x => x.Id, x => new InflectWordExerciseResult
				{
					DateTime = x.DateTime,
					FormResults = jsonSerializer.Deserialize<IReadOnlyCollection<InflectWordResult>>(x.FormResults),
				});

			return exercises.Select(x => new InflectWordExerciseData
				{
					ExerciseId = x.Id.ToItemId(),
					DescriptionTemplateId = x.TemplateId?.ToItemId(),
					Description = x.Description,
					BaseForm = x.BaseForm,
					WordForms = jsonSerializer.Deserialize<IReadOnlyCollection<InflectWordForm>>(x.WordForms),
					ExerciseResults = results[x.Id].ToList(),
					CreationTimestamp = x.CreationTimestamp,
				})
				.ToList();
		}

		public async Task AddInflectWordExercise(SaveInflectWordExerciseData createExerciseData, CancellationToken cancellationToken)
		{
			var exerciseEntity = new InflectWordExerciseEntity
			{
				LanguageId = createExerciseData.LanguageId.ToInt32(),
				TemplateId = createExerciseData.DescriptionTemplateId?.ToInt32(),
				Description = createExerciseData.Description,
				BaseForm = createExerciseData.BaseForm,
				WordForms = jsonSerializer.Serialize(createExerciseData.WordForms),
				CreationTimestamp = createExerciseData.CreationTimestamp,
			};

			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			await dbContext.InflectWordExercises.AddAsync(exerciseEntity, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
