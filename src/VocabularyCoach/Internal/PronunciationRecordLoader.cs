using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Exceptions;
using VocabularyCoach.Interfaces;
using VocabularyCoach.Models;

namespace VocabularyCoach.Internal
{
	internal sealed class PronunciationRecordLoader : IPronunciationRecordLoader
	{
		private readonly IReadOnlyDictionary<string, RecordFormat> recordFormats = new Dictionary<string, RecordFormat>(StringComparer.OrdinalIgnoreCase)
		{
			{ ".mp3", RecordFormat.Mp3 },
			{ ".oga", RecordFormat.Oga },
			{ ".ogg", RecordFormat.Oga },
			{ ".wav", RecordFormat.Wav },
		};

		private readonly IContentDownloader contentDownloader;

		private readonly IPronunciationRecordPlayer pronunciationRecordPlayer;

		public PronunciationRecordLoader(IContentDownloader contentDownloader, IPronunciationRecordPlayer pronunciationRecordPlayer)
		{
			this.contentDownloader = contentDownloader ?? throw new ArgumentNullException(nameof(contentDownloader));
			this.pronunciationRecordPlayer = pronunciationRecordPlayer ?? throw new ArgumentNullException(nameof(pronunciationRecordPlayer));
		}

		public async Task<PronunciationRecord> LoadPronunciationRecord(string dataSource, CancellationToken cancellationToken)
		{
			if (!Uri.TryCreate(dataSource, UriKind.Absolute, out var pronunciationRecordUrl))
			{
				throw new PronunciationRecordLoadException("The URL format is incorrect. Only HTTP URLs are supported (http:// or https://)");
			}

			if (pronunciationRecordUrl.Scheme != Uri.UriSchemeHttp && pronunciationRecordUrl.Scheme != Uri.UriSchemeHttps)
			{
				throw new PronunciationRecordLoadException("The URL scheme is incorrect. Only HTTP URLs are supported: http:// or https://");
			}

			var extension = Path.GetExtension(pronunciationRecordUrl.OriginalString);
			if (String.IsNullOrEmpty(extension))
			{
				throw new PronunciationRecordLoadException("Cannot detect record format from URL");
			}

			if (!recordFormats.TryGetValue(extension, out var recordFormat))
			{
				throw new PronunciationRecordLoadException($"File extension is not supported: {extension}");
			}

			byte[] recordData;

			try
			{
				recordData = await contentDownloader.Download(pronunciationRecordUrl, cancellationToken);
			}
			catch (ContentDownloadFailedException e)
			{
				throw new PronunciationRecordLoadException(e.Message, e);
			}

			var pronunciationRecord = new PronunciationRecord
			{
				Data = recordData,
				Format = recordFormat,
				Source = pronunciationRecordUrl.OriginalString,
			};

			if (!pronunciationRecordPlayer.PronunciationRecordDataIsCorrect(pronunciationRecord, out var dataError))
			{
				throw new PronunciationRecordLoadException($"Record data is incorrect: {dataError}");
			}

			return pronunciationRecord;
		}
	}
}
