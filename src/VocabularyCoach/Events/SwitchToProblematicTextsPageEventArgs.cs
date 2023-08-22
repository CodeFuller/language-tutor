using System;
using VocabularyCoach.Models;

namespace VocabularyCoach.Events
{
	internal class SwitchToProblematicTextsPageEventArgs : EventArgs
	{
		public Language StudiedLanguage { get; }

		public Language KnownLanguage { get; }

		public SwitchToProblematicTextsPageEventArgs(Language studiedLanguage, Language knownLanguage)
		{
			StudiedLanguage = studiedLanguage ?? throw new ArgumentNullException(nameof(studiedLanguage));
			KnownLanguage = knownLanguage ?? throw new ArgumentNullException(nameof(knownLanguage));
		}
	}
}
