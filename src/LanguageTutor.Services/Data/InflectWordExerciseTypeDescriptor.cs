using System.Collections.Generic;

namespace LanguageTutor.Services.Data
{
	public class InflectWordExerciseTypeDescriptor
	{
		public string Title { get; init; }

		public InflectWordExerciseDescriptionTemplate DescriptionTemplate { get; init; }

		public IReadOnlyCollection<string> FormHints { get; init; }

		public string GetDescription(string baseForm)
		{
			return DescriptionTemplate.GetDescription(baseForm);
		}
	}
}
