using System;
using System.Collections.Generic;
using System.IO;
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

		public Task<IReadOnlyCollection<StudiedTextWithTranslation>> GetStudiedTexts(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var polishLanguage = languages.Single(x => x.Name == "Polish");
			var russianLanguage = languages.Single(x => x.Name == "Russian");

			var result = new[]
			{
				new StudiedTextWithTranslation
				{
					StudiedText = new StudiedText
					{
						LanguageText = new LanguageText
						{
							Id = "44ccc4c5-f504-42b6-86fb-c35671e0722d",
							Language = polishLanguage,
							Text = "dziękuję",
						},
					},

					TextInKnownLanguage = new LanguageText
					{
						Id = "620399f4-4c7a-4aa2-87d2-8caf303a425c",
						Language = russianLanguage,
						Text = "спасибо",
					},
				},

				new StudiedTextWithTranslation
				{
					StudiedText = new StudiedText
					{
						LanguageText = new LanguageText
						{
							Id = "942d7063-4a35-4ca1-837f-9157fde72555",
							Language = polishLanguage,
							Text = "cześć",
						},
					},

					TextInKnownLanguage = new LanguageText
					{
						Id = "76660146-9c25-447f-be17-3bf2aa341eb4",
						Language = russianLanguage,
						Text = "привет",
					},
				},
			};

			result[0].StudiedText.AddCheckResult(new CheckResult
			{
				DateTime = new DateTime(2023, 07, 04, 8, 19, 36, DateTimeKind.Local),
				CheckResultType = CheckResultType.Ok,
			});

			return Task.FromResult<IReadOnlyCollection<StudiedTextWithTranslation>>(result);
		}

		public Task<CheckResultType> CheckTypedText(StudiedText studiedText, string typedText, CancellationToken cancellationToken)
		{
			var checkResult = new CheckResult
			{
				DateTime = DateTimeOffset.Now,
				CheckResultType = GetCheckResultType(studiedText.LanguageText, typedText),
			};

			studiedText.AddCheckResult(checkResult);

			return Task.FromResult(checkResult.CheckResultType);
		}

		public async Task<PronunciationRecord> GetPronunciationRecord(string textId, CancellationToken cancellationToken)
		{
			var filePath = $@"c:\temp\pronunciation\{textId}.oga";

			return new PronunciationRecord
			{
				Data = await File.ReadAllBytesAsync(filePath, cancellationToken),
				Format = RecordFormat.Oga,
			};
		}

		private static CheckResultType GetCheckResultType(LanguageText languageText, string typedText)
		{
			if (String.IsNullOrEmpty(typedText))
			{
				return CheckResultType.Skipped;
			}

			return String.Equals(languageText.Text, typedText, StringComparison.OrdinalIgnoreCase)
				? CheckResultType.Ok
				: CheckResultType.Misspelled;
		}
	}
}
