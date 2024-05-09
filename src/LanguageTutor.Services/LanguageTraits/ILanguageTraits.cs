using System;
using LanguageTutor.Models;
using LanguageTutor.Services.GoogleTextToSpeech.DataContracts;

namespace LanguageTutor.Services.LanguageTraits
{
	internal interface ILanguageTraits
	{
		Language Language { get; }

		Uri GetUrlForSpellCheck(string text);

		VoiceSelectionParams GetSynthesisVoiceConfiguration();
	}
}
