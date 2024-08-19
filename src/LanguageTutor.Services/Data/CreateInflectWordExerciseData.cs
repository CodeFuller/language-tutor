using System.Collections.Generic;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises.Inflection;

namespace LanguageTutor.Services.Data
{
	public class CreateInflectWordExerciseData
	{
		public ItemId LanguageId { get; init; }

		public InflectWordExerciseTypeDescriptor ExerciseTypeDescriptor { get; init; }

		public string Description { get; init; }

		public string BaseForm { get; init; }

		public IReadOnlyCollection<InflectWordForm> WordForms { get; init; }
	}
}
