using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Infrastructure.Sqlite.Entities;
using LanguageTutor.Infrastructure.Sqlite.Extensions;
using LanguageTutor.Infrastructure.Sqlite.Internal;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LanguageTutor.Infrastructure.Sqlite.Repositories
{
	internal sealed class ExerciseResultRepository : IExerciseResultRepository
	{
		private readonly IDbContextFactory<LanguageTutorDbContext> contextFactory;

		private readonly IJsonSerializer jsonSerializer;

		public ExerciseResultRepository(IDbContextFactory<LanguageTutorDbContext> contextFactory, IJsonSerializer jsonSerializer)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
		}

		public async Task AddTranslateTextExerciseResult(ItemId userId, TranslateTextExercise exercise, TranslateTextExerciseResult exerciseResult, CancellationToken cancellationToken)
		{
			var exerciseResultEntity = new TranslateTextExerciseResultEntity
			{
				UserId = userId.ToInt32(),
				TextId = exercise.TextInStudiedLanguage.Id.ToInt32(),
				DateTime = exerciseResult.DateTime,
				ResultType = exerciseResult.ResultType,
				TypedText = exerciseResult.TypedText,
			};

			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			await dbContext.TranslateTextExerciseResults.AddAsync(exerciseResultEntity, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task AddInflectWordExerciseResult(ItemId userId, InflectWordExercise exercise, InflectWordExerciseResult exerciseResult, CancellationToken cancellationToken)
		{
			var exerciseResultEntity = new InflectWordExerciseResultEntity
			{
				UserId = userId.ToInt32(),
				ExerciseId = exercise.Id.ToInt32(),
				DateTime = exerciseResult.DateTime,
				FormResults = jsonSerializer.Serialize(exerciseResult.FormResults),
			};

			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			await dbContext.InflectWordExerciseResults.AddAsync(exerciseResultEntity, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
