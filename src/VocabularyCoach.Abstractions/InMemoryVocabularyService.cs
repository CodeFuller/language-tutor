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
			var result = new[]
			{
				new StudiedWordOrPhraseWithTranslation
				{
					StudiedWordOrPhrase = new StudiedWordOrPhrase
					{
						WordOrPhrase = new WordOrPhrase
						{
							Id = "44ccc4c5-f504-42b6-86fb-c35671e0722d",
							Language = languages.Single(x => x.Name == "Polish"),
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
						Language = languages.Single(x => x.Name == "Russian"),
						Text = "спасибо",
					},
				},
			};

			return Task.FromResult<IReadOnlyCollection<StudiedWordOrPhraseWithTranslation>>(result);
		}
	}
}
