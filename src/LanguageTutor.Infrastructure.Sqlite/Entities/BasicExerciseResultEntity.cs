using System;

namespace LanguageTutor.Infrastructure.Sqlite.Entities
{
	internal abstract class BasicExerciseResultEntity
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public DateTimeOffset DateTime { get; set; }
	}
}
