using System;
using System.Collections.Generic;
using System.Linq;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Extensions;

namespace VocabularyCoach.Services.Internal
{
	internal class TextsForPracticeSelector : ITextsForPracticeSelector
	{
		private readonly INextCheckDateProvider nextCheckDateProvider;

		public TextsForPracticeSelector(INextCheckDateProvider nextCheckDateProvider)
		{
			this.nextCheckDateProvider = nextCheckDateProvider ?? throw new ArgumentNullException(nameof(nextCheckDateProvider));
		}

		public IReadOnlyCollection<StudiedText> GetTextsForPractice(DateOnly date, IEnumerable<StudiedText> studiedTexts, int dailyLimit)
		{
			var studiedTextsList = studiedTexts.ToList();

			var numberOfAlreadyPracticedTextsForDate = studiedTextsList.Count(x => x.CheckResults.Any(y => y.DateTime.ToDateOnly() == date));
			var numberOfRestTextsForPractice = dailyLimit - numberOfAlreadyPracticedTextsForDate;

			if (numberOfRestTextsForPractice <= 0)
			{
				return Array.Empty<StudiedText>();
			}

			var selectedTexts = new List<StudiedText>();

			// Taking all texts that were practiced before.
			var selectedPreviouslyPracticedTexts = studiedTextsList
				.Where(x => x.CheckResults.Any())
				.Select(x => new
				{
					StudiedText = x,
					NextCheckDateTime = nextCheckDateProvider.GetNextCheckDate(x),
				})
				.Where(x => x.NextCheckDateTime <= date)
				.GroupBy(x => x.NextCheckDateTime, x => x.StudiedText)
				.OrderBy(x => x.Key)
				.SelectMany(x => x.Randomize());

			selectedTexts.AddRange(selectedPreviouslyPracticedTexts);

			if (selectedTexts.Count < numberOfRestTextsForPractice)
			{
				// Taking first texts that were not practiced before.
				var selectedPreviouslyUnpracticedTexts = studiedTextsList
					.Where(x => !x.CheckResults.Any())
					.OrderBy(x => x.TextInStudiedLanguage.CreationTimestamp)
					.Take(numberOfRestTextsForPractice - selectedTexts.Count);

				selectedTexts.AddRange(selectedPreviouslyUnpracticedTexts);
			}

			return selectedTexts
				.Take(numberOfRestTextsForPractice)
				.Randomize()
				.ToList();
		}
	}
}
