using System;
using VocabularyCoach.Models;
using VocabularyCoach.Services.GoogleTextToSpeech.DataContracts;

namespace VocabularyCoach.Services.LanguageTraits
{
	internal interface ILanguageTraits
	{
		Language Language { get; }

		Uri GetUrlForTextCheck(string text);

		VoiceSelectionParams GetSynthesisVoiceConfiguration();
	}
}
