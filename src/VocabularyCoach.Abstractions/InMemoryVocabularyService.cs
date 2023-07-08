using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Abstractions.Data;
using VocabularyCoach.Abstractions.Interfaces;
using VocabularyCoach.Abstractions.LanguageTraits;
using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.Abstractions
{
	internal sealed class InMemoryVocabularyService : IVocabularyService, IEditVocabularyService
	{
		private readonly IReadOnlyCollection<Language> languages = new List<Language>
		{
			SupportedLanguages.Polish,
			SupportedLanguages.Russian,
		};

		private readonly List<StudiedTextWithTranslation> studiedTexts = new();

		private readonly Dictionary<string, PronunciationRecord> pronunciationRecords = new();

		private readonly IReadOnlyDictionary<Language, ILanguageTraits> languagesTraits;

		public InMemoryVocabularyService(IEnumerable<ILanguageTraits> languagesTraits)
		{
			this.languagesTraits = languagesTraits?.ToDictionary(x => x.Language, x => x) ?? throw new ArgumentNullException(nameof(languagesTraits));

			var initialStudiedTexts = new[]
			{
				new StudiedTextWithTranslation
				{
					StudiedText = new StudiedText
					{
						LanguageText = new LanguageText
						{
							Id = "44ccc4c5-f504-42b6-86fb-c35671e0722d",
							Language = SupportedLanguages.Polish,
							Text = "dziękuję",
						},
					},

					TextInKnownLanguage = new LanguageText
					{
						Id = "620399f4-4c7a-4aa2-87d2-8caf303a425c",
						Language = SupportedLanguages.Russian,
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
							Language = SupportedLanguages.Polish,
							Text = "cześć",
						},
					},

					TextInKnownLanguage = new LanguageText
					{
						Id = "76660146-9c25-447f-be17-3bf2aa341eb4",
						Language = SupportedLanguages.Russian,
						Text = "привет",
					},
				},
			};

			initialStudiedTexts[0].StudiedText.AddCheckResult(new CheckResult
			{
				DateTime = new DateTime(2023, 07, 04, 8, 19, 36, DateTimeKind.Local),
				CheckResultType = CheckResultType.Ok,
			});

			studiedTexts.AddRange(initialStudiedTexts);

			foreach (var text in initialStudiedTexts)
			{
				var filePath = $@"c:\temp\pronunciation\{text.StudiedText.LanguageText.Id}.oga";

				var pronunciationRecord = new PronunciationRecord
				{
					Data = File.ReadAllBytes(filePath),
					Format = RecordFormat.Oga,
				};

				pronunciationRecords.Add(text.StudiedText.LanguageText.Id, pronunciationRecord);
			}
		}

		public Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken)
		{
			return Task.FromResult(languages);
		}

		public Task<IReadOnlyCollection<StudiedTextWithTranslation>> GetStudiedTexts(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			return Task.FromResult<IReadOnlyCollection<StudiedTextWithTranslation>>(studiedTexts);
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

		public Task<PronunciationRecord> GetPronunciationRecord(string textId, CancellationToken cancellationToken)
		{
			return Task.FromResult(pronunciationRecords[textId]);
		}

		public Task<IReadOnlyCollection<LanguageText>> GetLanguageTexts(Language language, CancellationToken cancellationToken)
		{
			return Task.FromResult<IReadOnlyCollection<LanguageText>>(studiedTexts.Select(x => x.StudiedText.LanguageText).ToList());
		}

		public async Task<Uri> GetUrlForLanguageTextCheck(LanguageText languageText, CancellationToken cancellationToken)
		{
			if (!languagesTraits.TryGetValue(languageText.Language, out var languageTraits))
			{
				throw new NotSupportedException($"Language is not supported: {languageText.Language}");
			}

			return await languageTraits.GetUrlForTextCheck(languageText.Text, cancellationToken);
		}

		public Task<LanguageText> AddOrUpdateLanguageTextWithTranslation(LanguageTextCreationData languageTextData1, LanguageTextCreationData languageTextData2, CancellationToken cancellationToken)
		{
			var languageText1 = new LanguageText
			{
				Id = Guid.NewGuid().ToString("B"),
				Language = languageTextData1.Language,
				Text = languageTextData1.Text,
				Note = languageTextData1.Note,
			};

			var languageText2 = new LanguageText
			{
				Id = Guid.NewGuid().ToString("B"),
				Language = languageTextData2.Language,
				Text = languageTextData2.Text,
				Note = languageTextData2.Note,
			};

			var studiedTextWithTranslation = new StudiedTextWithTranslation
			{
				StudiedText = new StudiedText
				{
					LanguageText = languageText1,
				},

				TextInKnownLanguage = languageText2,
			};

			studiedTexts.Add(studiedTextWithTranslation);

			if (languageTextData1.PronunciationRecord != null)
			{
				pronunciationRecords.Add(languageText1.Id, languageTextData1.PronunciationRecord);
			}

			if (languageTextData2.PronunciationRecord != null)
			{
				pronunciationRecords.Add(languageText2.Id, languageTextData2.PronunciationRecord);
			}

			return Task.FromResult(languageText1);
		}
	}
}
