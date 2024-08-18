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

		public ExerciseResultRepository(IDbContextFactory<LanguageTutorDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task AddTranslateTextExerciseResult(ItemId userId, TranslateTextExercise exercise, TranslateTextExerciseResult exerciseResult, CancellationToken cancellationToken)
		{
			var exerciseResultEntity = new TranslateTextExerciseResultEntity
			{
				Id = exerciseResult.Id?.ToInt32() ?? default,
				UserId = userId.ToInt32(),
				TextId = exercise.TextInStudiedLanguage.Id.ToInt32(),
				DateTime = exerciseResult.DateTime,
				ResultType = exerciseResult.ResultType,
				TypedText = exerciseResult.TypedText,
			};

			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			await dbContext.TranslateTextExerciseResults.AddAsync(exerciseResultEntity, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);

			exerciseResult.Id = exerciseResultEntity.Id.ToItemId();
		}

		public Task AddInflectWordExerciseResult(ItemId userId, InflectWordExercise exercise, InflectWordExerciseResult exerciseResult, CancellationToken cancellationToken)
		{
			// TODO: Implement DAL for InflectWordExercise.
			return Task.CompletedTask;
		}
	}
}
