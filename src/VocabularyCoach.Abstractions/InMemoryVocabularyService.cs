using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Abstractions.Interfaces;
using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.Abstractions
{
	internal sealed class InMemoryVocabularyService : IVocabularyService
	{
		private readonly IReadOnlyCollection<Language> languages = new List<Language>
		{
			new()
			{
				Id = "f7013078-e287-4dd1-8f09-4e16a9d2f4d4",
				Name = "Polish",
			},

			new()
			{
				Id = "454c0e62-0cc8-4e33-9977-00b06ca309d2",
				Name = "Russian",
			},
		};

		public Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken)
		{
			return Task.FromResult(languages);
		}

		public Task<IReadOnlyCollection<StudiedWordOrPhraseWithTranslation>> GetStudiedWords(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var polishLanguage = languages.Single(x => x.Name == "Polish");
			var russianLanguage = languages.Single(x => x.Name == "Russian");

			var result = new[]
			{
				new StudiedWordOrPhraseWithTranslation
				{
					StudiedWordOrPhrase = new StudiedWordOrPhrase
					{
						WordOrPhrase = new WordOrPhrase
						{
							Id = "44ccc4c5-f504-42b6-86fb-c35671e0722d",
							Language = polishLanguage,
							Text = "dziękuję",
						},

						CheckResults = new[]
						{
							new CheckResult
							{
								DateTime = new DateTime(2023, 07, 04, 8, 19, 36, DateTimeKind.Local),
								CheckResultType = CheckResultType.Ok,
							},
						},
					},

					WordOrPhraseInKnownLanguage = new WordOrPhrase
					{
						Id = "620399f4-4c7a-4aa2-87d2-8caf303a425c",
						Language = russianLanguage,
						Text = "спасибо",
					},
				},

				new StudiedWordOrPhraseWithTranslation
				{
					StudiedWordOrPhrase = new StudiedWordOrPhrase
					{
						WordOrPhrase = new WordOrPhrase
						{
							Id = "942d7063-4a35-4ca1-837f-9157fde72555",
							Language = polishLanguage,
							Text = "cześć",
						},
					},

					WordOrPhraseInKnownLanguage = new WordOrPhrase
					{
						Id = "76660146-9c25-447f-be17-3bf2aa341eb4",
						Language = russianLanguage,
						Text = "привет",
					},
				},
			};

			return Task.FromResult<IReadOnlyCollection<StudiedWordOrPhraseWithTranslation>>(result);
		}

		public Task<CheckResultType> CheckTypedWordOrPhrase(StudiedWordOrPhrase studiedWordOrPhrase, string typedWordOrPhrase, CancellationToken cancellationToken)
		{
			var checkResult = new CheckResult
			{
				DateTime = DateTimeOffset.Now,
				CheckResultType = GetCheckResultType(studiedWordOrPhrase.WordOrPhrase, typedWordOrPhrase),
			};

			studiedWordOrPhrase.AddCheckResult(checkResult);

			return Task.FromResult(checkResult.CheckResultType);
		}

		private static CheckResultType GetCheckResultType(WordOrPhrase wordOrPhrase, string typedWordOrPhrase)
		{
			if (String.IsNullOrEmpty(typedWordOrPhrase))
			{
				return CheckResultType.Skipped;
			}

			return String.Equals(wordOrPhrase.Text, typedWordOrPhrase, StringComparison.OrdinalIgnoreCase)
				? CheckResultType.Ok
				: CheckResultType.Misspelled;
		}
	}
}
