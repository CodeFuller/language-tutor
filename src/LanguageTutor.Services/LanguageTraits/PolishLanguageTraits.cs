using System;
using LanguageTutor.Models;
using LanguageTutor.Services.GoogleTextToSpeech.DataContracts;

namespace LanguageTutor.Services.LanguageTraits
{
	internal sealed class PolishLanguageTraits : ILanguageTraits
	{
		public Language Language => SupportedLanguages.Polish;

		public Uri GetUrlForSpellCheck(string text)
		{
			return new Uri($"https://pl.wiktionary.org/wiki/{text}");
		}

		public VoiceSelectionParams GetSynthesisVoiceConfiguration()
		{
			return new VoiceSelectionParams
			{
				LanguageCode = "pl-PL",
				Name = "pl-PL-Wavenet-A",
				SsmlGender = "FEMALE",
			};
		}
	}
}
