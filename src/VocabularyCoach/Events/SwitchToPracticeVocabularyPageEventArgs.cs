using System;
using VocabularyCoach.Models;

namespace VocabularyCoach.Events
{
	internal sealed class SwitchToPracticeVocabularyPageEventArgs : EventArgs
	{
		public Language StudiedLanguage { get; }

		public Language KnownLanguage { get; }

		public SwitchToPracticeVocabularyPageEventArgs(Language studiedLanguage, Language knownLanguage)
		{
			StudiedLanguage = studiedLanguage ?? throw new ArgumentNullException(nameof(studiedLanguage));
			KnownLanguage = knownLanguage ?? throw new ArgumentNullException(nameof(knownLanguage));
		}
	}
}
