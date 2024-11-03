using LanguageTutor.Models;

namespace LanguageTutor.Infrastructure.Sqlite.Entities
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

		// This navigation property was added to let EF Core know about foreign key dependency from PronunciationRecords table to Texts table.
		// Without it the MergeDatabases utility fails, because EF Core tries to add new PronunciationRecords entries before adding new Texts entries.
		// As result, the error "FOREIGN KEY constraint failed" is thrown.
		public TextEntity Text { get; set; }
	}
}
