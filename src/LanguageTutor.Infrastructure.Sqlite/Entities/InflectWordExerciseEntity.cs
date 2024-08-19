using System;

namespace LanguageTutor.Infrastructure.Sqlite.Entities
{
	internal sealed class InflectWordExerciseEntity
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public int? TemplateId { get; set; }

		public string Description { get; set; }

		public string BaseForm { get; set; }

		public string WordForms { get; set; }

		public DateTimeOffset CreationTimestamp { get; set; }
	}
}
