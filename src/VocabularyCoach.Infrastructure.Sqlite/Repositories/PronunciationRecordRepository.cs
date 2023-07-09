using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VocabularyCoach.Infrastructure.Sqlite.Entities;
using VocabularyCoach.Infrastructure.Sqlite.Extensions;
using VocabularyCoach.Infrastructure.Sqlite.Internal;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Interfaces.Repositories;

namespace VocabularyCoach.Infrastructure.Sqlite.Repositories
{
	internal sealed class PronunciationRecordRepository : IPronunciationRecordRepository
	{
		private readonly IDbContextFactory<VocabularyCoachDbContext> contextFactory;

		private readonly VocabularyDatabaseSettings settings;

		public PronunciationRecordRepository(IDbContextFactory<VocabularyCoachDbContext> contextFactory, IOptions<VocabularyDatabaseSettings> options)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<PronunciationRecord> GetPronunciationRecord(ItemId languageTextId, CancellationToken cancellationToken)
		{
			await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

			var pronunciationRecordEntity = await context.PronunciationRecords.SingleOrDefaultAsync(x => x.TextId == languageTextId.ToInt32(), cancellationToken);

			if (pronunciationRecordEntity == null)
			{
				return null;
			}

			var recordDataPath = GetPathForPronunciationRecordDataFile(pronunciationRecordEntity);
			var recordData = await File.ReadAllBytesAsync(recordDataPath, cancellationToken);

			return new PronunciationRecord
			{
				Data = recordData,
				Format = pronunciationRecordEntity.Format,
				Source = pronunciationRecordEntity.Source,
			};
		}

		public async Task AddPronunciationRecord(ItemId languageTextId, PronunciationRecord pronunciationRecord, CancellationToken cancellationToken)
		{
			var pronunciationRecordEntity = new PronunciationRecordEntity
			{
				TextId = languageTextId.ToInt32(),
				Format = pronunciationRecord.Format,
				Source = pronunciationRecord.Source,
				Path = Path.ChangeExtension(Guid.NewGuid().ToString("D"), ".bin"),
			};

			var recordDataPath = GetPathForPronunciationRecordDataFile(pronunciationRecordEntity);
			await File.WriteAllBytesAsync(recordDataPath, pronunciationRecord.Data, cancellationToken);

			await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

			await context.PronunciationRecords.AddAsync(pronunciationRecordEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);
		}

		private string GetPathForPronunciationRecordDataFile(PronunciationRecordEntity pronunciationRecordEntity)
		{
			return Path.Combine(settings.PronunciationRecordsPath, pronunciationRecordEntity.Path);
		}
	}
}
