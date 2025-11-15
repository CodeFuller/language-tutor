using System;
using System.Collections.Generic;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.GoogleTextToSpeech.DataContracts;

namespace LanguageTutor.Services.LanguageTraits
{
	internal class EnglishLanguageTraits : ILanguageTraits
	{
		public Language Language => SupportedLanguages.English;

		public Uri GetUrlForSpellCheck(string text)
		{
			return new Uri($"https://en.wiktionary.org/wiki/{text}");
		}

		public VoiceSelectionParams GetSynthesisVoiceConfiguration()
		{
			return new VoiceSelectionParams
			{
				LanguageCode = "en-US",
				Name = "en-US-Wavenet-C",
				SsmlGender = "FEMALE",
			};
		}

		public bool InflectWordExercisesAreSupported()
		{
			return false;
		}

		public IReadOnlyCollection<InflectWordExerciseTypeDescriptor> GetInflectWordExerciseTypes()
		{
			return [];
		}
	}
}
