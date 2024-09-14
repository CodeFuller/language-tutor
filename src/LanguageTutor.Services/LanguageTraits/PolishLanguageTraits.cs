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

			var masculineVerbFormHints = new[]
			{
				"ja",
				"ty",
				"on",
				"my",
				"wy",
				"oni",
			};

			var feminineVerbFormHints = new[]
			{
				"ja",
				"ty",
				"ona",
				"my",
				"wy",
				"one",
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

			var comparisonFormHints = new[]
			{
				"równy",
				"wyższy",
				"najwyższy",
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
					Title = "Odmiana czasownika w czasie przeszłym, r.m.",
					DescriptionTemplate = new()
					{
						Id = new("2"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w czasie przeszłym, r.m.",
					},
					FormHints = masculineVerbFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana czasownika w czasie przeszłym, r.ż.",
					DescriptionTemplate = new()
					{
						Id = new("3"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w czasie przeszłym, r.ż.",
					},
					FormHints = feminineVerbFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana czasownika w czasie przyszłym",
					DescriptionTemplate = new()
					{
						Id = new("11"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w czasie przyszłym",
					},
					FormHints = verbFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana czasownika w trybie rozkazującym",
					DescriptionTemplate = new()
					{
						Id = new("4"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w trybie rozkazującym",
					},
					FormHints = verbFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana czasownika w trybie przypuszczającym, r.m.",
					DescriptionTemplate = new()
					{
						Id = new("12"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w trybie przypuszczającym, r.m.",
					},
					FormHints = masculineVerbFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana czasownika w trybie przypuszczającym, r.ż.",
					DescriptionTemplate = new()
					{
						Id = new("13"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w trybie przypuszczającym, r.ż.",
					},
					FormHints = feminineVerbFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana czasownika w trybie oznajmującym, r.m.",
					DescriptionTemplate = new()
					{
						Id = new("14"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w trybie oznajmującym, r.m.",
					},
					FormHints = masculineVerbFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana czasownika w trybie oznajmującym, r.ż.",
					DescriptionTemplate = new()
					{
						Id = new("15"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w trybie oznajmującym, r.ż.",
					},
					FormHints = feminineVerbFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana słowa przez przypadki",
					DescriptionTemplate = new()
					{
						Id = new("5"),
						Template = "Proszę odmienić słowo \"{BaseForm}\" przez przypadki",
					},
					FormHints = caseFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana frazy przez przypadki",
					DescriptionTemplate = new()
					{
						Id = new("9"),
						Template = "Proszę odmienić frazę \"{BaseForm}\" przez przypadki",
					},
					FormHints = caseFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana zaimka przez przypadki",
					DescriptionTemplate = new()
					{
						Id = new("8"),
						Template = "Proszę odmienić zaimek \"{BaseForm}\" przez przypadki",
					},
					FormHints =
					[
						"mianownik",
						"dopełniacz",
						"celownik",
						"biernik",
						"narzędnik",
						"miejscownik",
					],
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Stopniowanie przymiotników",
					DescriptionTemplate = new()
					{
						Id = new("6"),
						Template = "Proszę odmienić przymiotnik \"{BaseForm}\" w stopniach porównania",
					},
					FormHints = comparisonFormHints,
				},

				new InflectWordExerciseTypeDescriptor
				{
					Title = "Stopniowanie przysłówków",
					DescriptionTemplate = new()
					{
						Id = new("7"),
						Template = "Proszę odmienić przysłówiek \"{BaseForm}\" w stopniach porównania",
					},
					FormHints = comparisonFormHints,
				},
			];
		}
	}
}
