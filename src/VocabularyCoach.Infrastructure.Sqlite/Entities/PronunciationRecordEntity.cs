using VocabularyCoach.Models;

namespace VocabularyCoach.Infrastructure.Sqlite.Entities
{
	internal sealed class PronunciationRecordEntity
	{
		public int Id { get; set; }

		public int TextId { get; set; }

		public RecordFormat Format { get; set; }

		public string Source { get; set; }

		public string Path { get; set; }

		public int DataLength { get; set; }

		public int DataChecksum { get; set; }
	}
}
