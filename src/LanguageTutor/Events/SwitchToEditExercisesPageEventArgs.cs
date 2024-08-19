using System;
using LanguageTutor.Models;

namespace LanguageTutor.Events
{
	internal class SwitchToEditExercisesPageEventArgs : EventArgs
	{
		public Language StudiedLanguage { get; }

		public SwitchToEditExercisesPageEventArgs(Language studiedLanguage)
		{
			StudiedLanguage = studiedLanguage ?? throw new ArgumentNullException(nameof(studiedLanguage));
		}
	}
}
