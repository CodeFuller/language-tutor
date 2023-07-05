using System;
using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.Events
{
	internal sealed class SwitchToStudyVocabularyPageEventArgs : EventArgs
	{
		public Language StudiedLanguage { get; }

		public Language KnownLanguage { get; }

		public SwitchToStudyVocabularyPageEventArgs(Language studiedLanguage, Language knownLanguage)
		{
			StudiedLanguage = studiedLanguage ?? throw new ArgumentNullException(nameof(studiedLanguage));
			KnownLanguage = knownLanguage ?? throw new ArgumentNullException(nameof(knownLanguage));
		}
	}
}
