using System;
using System.Collections.Generic;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Models.Exercises.Inflection;

namespace LanguageTutor.Services.Data
{
	public class InflectWordExerciseData
	{
		public DateTimeOffset CreationTimestamp { get; init; }

		public string Description { get; init; }

		public ItemId DescriptionTemplateId { get; init; }

		public string BaseForm { get; init; }

		public IReadOnlyCollection<InflectWordForm> WordForms { get; init; }

		public IReadOnlyCollection<InflectWordExerciseResult> ExerciseResults { get; init; }
	}
}
