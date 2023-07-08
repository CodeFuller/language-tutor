using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Vorbis;
using NAudio.Wave;
using VocabularyCoach.Abstractions.Models;
using VocabularyCoach.Interfaces;

namespace VocabularyCoach.Internal
{
	internal sealed class PronunciationRecordPlayer : IPronunciationRecordPlayer
	{
		public async Task PlayPronunciationRecord(PronunciationRecord pronunciationRecord, CancellationToken cancellationToken)
		{
			using var memoryStream = new MemoryStream(pronunciationRecord.Data);
			await using var fileReader = CreateRecordReader(memoryStream, pronunciationRecord.Format);

			using var outputDevice = new WaveOutEvent();
			outputDevice.Init(fileReader);
			outputDevice.Play();

			while (outputDevice.PlaybackState == PlaybackState.Playing)
			{
				await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
			}
		}

		private static WaveStream CreateRecordReader(Stream dataStream, RecordFormat format)
		{
			return format switch
			{
				RecordFormat.Mp3 => new Mp3FileReader(dataStream),
				RecordFormat.Oga => new VorbisWaveReader(dataStream),
				_ => throw new NotSupportedException($"Record format is not supported: {format}"),
			};
		}
	}
}
