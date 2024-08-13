using System;
using LanguageTutor.Models;
using LanguageTutor.ViewModels.Data;

namespace LanguageTutor.Events
{
	internal sealed class SwitchToExerciseResultsPageEventArgs : EventArgs
	{
		public Language StudiedLanguage { get; }

		public Language KnownLanguage { get; }

		public ExerciseResults ExerciseResults { get; }

		public SwitchToExerciseResultsPageEventArgs(Language studiedLanguage, Language knownLanguage, ExerciseResults exerciseResults)
		{
			StudiedLanguage = studiedLanguage ?? throw new ArgumentNullException(nameof(studiedLanguage));
			KnownLanguage = knownLanguage ?? throw new ArgumentNullException(nameof(knownLanguage));
			ExerciseResults = exerciseResults ?? throw new ArgumentNullException(nameof(exerciseResults));
		}
	}
}
