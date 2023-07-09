namespace VocabularyCoach.Models
{
	public class PronunciationRecord
	{
#pragma warning disable CA1819 // Properties should not return arrays
		public byte[] Data { get; init; }
#pragma warning restore CA1819 // Properties should not return arrays

		public RecordFormat Format { get; init; }

		public string Source { get; init; }
	}
}
