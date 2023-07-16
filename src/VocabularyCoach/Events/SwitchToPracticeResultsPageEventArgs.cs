using System;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels.Data;

namespace VocabularyCoach.Events
{
	internal sealed class SwitchToPracticeResultsPageEventArgs : EventArgs
	{
		public Language StudiedLanguage { get; }

		public Language KnownLanguage { get; }

		public PracticeResults PracticeResults { get; }

		public SwitchToPracticeResultsPageEventArgs(Language studiedLanguage, Language knownLanguage, PracticeResults practiceResults)
		{
			StudiedLanguage = studiedLanguage ?? throw new ArgumentNullException(nameof(studiedLanguage));
			KnownLanguage = knownLanguage ?? throw new ArgumentNullException(nameof(knownLanguage));
			PracticeResults = practiceResults ?? throw new ArgumentNullException(nameof(practiceResults));
		}
	}
}
