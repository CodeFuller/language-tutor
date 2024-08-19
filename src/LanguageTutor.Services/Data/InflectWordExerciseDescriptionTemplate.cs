using System;
using LanguageTutor.Models;

namespace LanguageTutor.Services.Data
{
	public class InflectWordExerciseDescriptionTemplate
	{
		public ItemId Id { get; init; }

		public string Template { get; init; }

		public string GetDescription(string baseForm)
		{
			return Template.Replace("{BaseForm}", baseForm, StringComparison.Ordinal);
		}
	}
}
