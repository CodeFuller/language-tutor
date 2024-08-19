using System;
using System.Collections.Generic;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
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

		public bool InflectWordExercisesAreSupported()
		{
			return true;
		}

		public IReadOnlyCollection<InflectWordExerciseTypeDescriptor> GetInflectWordExerciseTypes()
		{
			var verbFormHints = new[]
			{
				"ja",
				"ty",
				"on, ona, ono",
				"my",
				"wy",
				"oni, one",
			};

			var caseFormHints = new[]
			{
				"mianownik",
				"dopełniacz",
				"celownik",
				"biernik",
				"narzędnik",
				"miejscownik",
				"wołacz",
			};

			return
			[
				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana czasownika w czasie teraźniejszym",
					DescriptionTemplate = new()
					{
						Id = new("1"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w czasie teraźniejszym",
					},
					FormHints = verbFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana czasownika w czasie przeszłym",
					DescriptionTemplate = new()
					{
						Id = new("2"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w czasie przeszłym",
					},
					FormHints = verbFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana czasownika w trybie rozkazującym",
					DescriptionTemplate = new()
					{
						Id = new("3"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w trybie rozkazującym",
					},
					FormHints = verbFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana słowa przez przypadki",
					DescriptionTemplate = new()
					{
						Id = new("4"),
						Template = "Proszę odmienić słowo \"{BaseForm}\" przez przypadki",
					},
					FormHints = caseFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Stopniowanie przymiotników i przysłówków",
					DescriptionTemplate = new()
					{
						Id = new("5"),
						Template = "Proszę odmienić słowo \"{BaseForm}\" w stopniach porównania",
					},
					FormHints =
					[
						"równy",
						"wyższy",
						"najwyższy",
					],
				},
			];
		}
	}
}
