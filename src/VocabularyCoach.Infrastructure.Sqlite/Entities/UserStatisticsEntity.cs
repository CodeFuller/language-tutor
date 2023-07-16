using System;

namespace VocabularyCoach.Infrastructure.Sqlite.Entities
{
	internal sealed class UserStatisticsEntity
	{
		public int UserId { get; set; }

		public int StudiedLanguageId { get; set; }

		public int KnownLanguageId { get; set; }

		public DateOnly Date { get; set; }

		public int TotalTextsNumber { get; set; }

		public int TotalLearnedTextsNumber { get; set; }

		public int RestNumberOfTextsToPractice { get; set; }

		public int NumberOfPracticedTexts { get; set; }
	}
}
