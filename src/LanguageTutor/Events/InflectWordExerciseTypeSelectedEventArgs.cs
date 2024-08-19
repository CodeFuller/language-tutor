using System;
using LanguageTutor.ViewModels;

namespace LanguageTutor.Events
{
	internal class InflectWordExerciseTypeSelectedEventArgs : EventArgs
	{
		public InflectWordExerciseTypeViewModel SelectedExerciseType { get; }

		public InflectWordExerciseTypeSelectedEventArgs(InflectWordExerciseTypeViewModel selectedExerciseType)
		{
			SelectedExerciseType = selectedExerciseType ?? throw new ArgumentNullException(nameof(selectedExerciseType));
		}
	}
}
