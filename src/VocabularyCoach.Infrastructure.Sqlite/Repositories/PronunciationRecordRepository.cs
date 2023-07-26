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

		private readonly IChecksumCalculator checksumCalculator;

		private readonly VocabularyDatabaseSettings settings;

		public PronunciationRecordRepository(IDbContextFactory<VocabularyCoachDbContext> contextFactory, IChecksumCalculator checksumCalculator, IOptions<VocabularyDatabaseSettings> options)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.checksumCalculator = checksumCalculator ?? throw new ArgumentNullException(nameof(checksumCalculator));
			settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<PronunciationRecord> GetPronunciationRecord(ItemId languageTextId, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var pronunciationRecordEntity = await dbContext.PronunciationRecords.SingleOrDefaultAsync(x => x.TextId == languageTextId.ToInt32(), cancellationToken);

			if (pronunciationRecordEntity == null)
			{
				return null;
			}

			var recordDataPath = GetPathForPronunciationRecordDataFile(pronunciationRecordEntity);
			var recordData = await File.ReadAllBytesAsync(recordDataPath, cancellationToken);

			if (recordData.Length != pronunciationRecordEntity.DataLength)
			{
				throw new InvalidOperationException($"Pronunciation record data is corrupted. Length {recordData.Length} != {pronunciationRecordEntity.DataLength}");
			}

			var actualChecksum = checksumCalculator.CalculateChecksum(recordData);
			var expectedChecksum = (uint)pronunciationRecordEntity.DataChecksum;
			if (actualChecksum != expectedChecksum)
			{
				throw new InvalidOperationException($"Pronunciation record data is corrupted. Checksum {actualChecksum:X8} != {expectedChecksum:X8}");
			}

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
			};

			FillPronunciationRecordEntity(pronunciationRecordEntity, pronunciationRecord);

			await StorePronunciationRecordData(pronunciationRecordEntity, pronunciationRecord, cancellationToken);

			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			await dbContext.PronunciationRecords.AddAsync(pronunciationRecordEntity, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task UpdatePronunciationRecord(ItemId languageTextId, PronunciationRecord pronunciationRecord, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var pronunciationRecordEntity = await dbContext.PronunciationRecords.SingleAsync(x => x.TextId == languageTextId.ToInt32(), cancellationToken);

			var oldDataFilePath = GetPathForPronunciationRecordDataFile(pronunciationRecordEntity);

			FillPronunciationRecordEntity(pronunciationRecordEntity, pronunciationRecord);

			await StorePronunciationRecordData(pronunciationRecordEntity, pronunciationRecord, cancellationToken);

			await dbContext.SaveChangesAsync(cancellationToken);

			File.Delete(oldDataFilePath);
		}

		private void FillPronunciationRecordEntity(PronunciationRecordEntity pronunciationRecordEntity, PronunciationRecord pronunciationRecord)
		{
			pronunciationRecordEntity.Format = pronunciationRecord.Format;
			pronunciationRecordEntity.Source = pronunciationRecord.Source;
			pronunciationRecordEntity.Path = Path.ChangeExtension(Guid.NewGuid().ToString("D"), GetExtensionForPronunciationRecordDataFile(pronunciationRecord));
			pronunciationRecordEntity.DataLength = pronunciationRecord.Data.Length;
			pronunciationRecordEntity.DataChecksum = (int)checksumCalculator.CalculateChecksum(pronunciationRecord.Data);
		}

		private async Task StorePronunciationRecordData(PronunciationRecordEntity pronunciationRecordEntity, PronunciationRecord pronunciationRecord, CancellationToken cancellationToken)
		{
			var recordDataPath = GetPathForPronunciationRecordDataFile(pronunciationRecordEntity);
			await File.WriteAllBytesAsync(recordDataPath, pronunciationRecord.Data, cancellationToken);
		}

		private static string GetExtensionForPronunciationRecordDataFile(PronunciationRecord pronunciationRecord)
		{
			return pronunciationRecord.Format switch
			{
				RecordFormat.Mp3 => ".mp3",
				RecordFormat.Oga => ".oga",
				RecordFormat.Wav => ".wav",
				_ => throw new NotSupportedException($"Pronunciation record format is not supported: {pronunciationRecord.Format}"),
			};
		}

		private string GetPathForPronunciationRecordDataFile(PronunciationRecordEntity pronunciationRecordEntity)
		{
			return Path.Combine(settings.PronunciationRecordsPath, pronunciationRecordEntity.Path);
		}
	}
}
