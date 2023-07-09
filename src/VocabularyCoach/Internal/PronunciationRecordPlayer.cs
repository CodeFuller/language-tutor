using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Vorbis;
using NAudio.Wave;
using VocabularyCoach.Interfaces;
using VocabularyCoach.Models;

namespace VocabularyCoach.Internal
{
	internal sealed class PronunciationRecordPlayer : IPronunciationRecordPlayer
	{
		public Task PlayPronunciationRecord(PronunciationRecord pronunciationRecord, CancellationToken cancellationToken)
		{
			// We play record in background task to prevent block of UI thread and allow user to invoke next UI command.
			// We do not use cancellation token (could be fired by AsyncRelayCommand), because cancellation will only prevent clean-up (dispose), which we want to avoid.
			cancellationToken = CancellationToken.None;

			Task.Run(
				async () =>
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
				}, cancellationToken);

			return Task.CompletedTask;
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
