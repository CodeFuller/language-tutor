using System;
using System.Collections.Generic;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.GoogleTextToSpeech.DataContracts;

namespace LanguageTutor.Services.LanguageTraits
{
	internal interface ILanguageTraits
	{
		Language Language { get; }

		Uri GetUrlForSpellCheck(string text);

		VoiceSelectionParams GetSynthesisVoiceConfiguration();

		bool InflectWordExercisesAreSupported();

		IReadOnlyCollection<InflectWordExerciseTypeDescriptor> GetInflectWordExerciseTypes();
	}
}
