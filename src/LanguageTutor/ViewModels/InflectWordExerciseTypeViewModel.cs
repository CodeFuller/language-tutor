using LanguageTutor.Services.Data;

namespace LanguageTutor.ViewModels
{
	public sealed class InflectWordExerciseTypeViewModel
	{
		public InflectWordExerciseTypeDescriptor ExerciseTypeDescriptor { get; }

		public string Title => ExerciseTypeDescriptor.Title;

		public InflectWordExerciseTypeViewModel(InflectWordExerciseTypeDescriptor exerciseTypeDescriptor)
		{
			ExerciseTypeDescriptor = exerciseTypeDescriptor;
		}

		public override string ToString()
		{
			return Title;
		}
	}
}
