using System;
using LanguageTutor.Models;

namespace LanguageTutor.Events
{
	internal sealed class SwitchToPerformExercisesPageEventArgs : EventArgs
	{
		public Language StudiedLanguage { get; }

		public Language KnownLanguage { get; }

		public SwitchToPerformExercisesPageEventArgs(Language studiedLanguage, Language knownLanguage)
		{
			StudiedLanguage = studiedLanguage ?? throw new ArgumentNullException(nameof(studiedLanguage));
			KnownLanguage = knownLanguage ?? throw new ArgumentNullException(nameof(knownLanguage));
		}
	}
}
