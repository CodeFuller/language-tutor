using System;
using LanguageTutor.Models;

namespace LanguageTutor.Infrastructure.Sqlite.Entities
{
	internal sealed class CheckResultEntity
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public int TextId { get; set; }

		public TextEntity Text { get; set; }

		public DateTimeOffset DateTime { get; set; }

		public CheckResultType ResultType { get; set; }

		public string TypedText { get; set; }
	}
}
