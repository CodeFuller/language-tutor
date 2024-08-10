using System;
using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models;
using LanguageTutor.Services.Extensions;

namespace LanguageTutor.Services.Internal
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

			var previouslyUnpracticedTexts = studiedTextsList
				.Where(x => !x.CheckResults.Any())
				.OrderBy(x => x.TextInStudiedLanguage.CreationTimestamp)
				.ToList();

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

			// The logic is the following:
			//
			//   If there are a lot of texts unpracticed before (i.e. this is a new user, who has a lot of texts to learn),
			//   then we add new texts gradually, only if she has learned previous texts already.
			//
			//   If there are not too much texts unpracticed before (i.e. this is an old user and just new texts were added to the dictionary),
			//   then we mix new texts with the practiced before.
			if (previouslyUnpracticedTexts.Count > dailyLimit)
			{
				selectedTexts.AddRange(selectedPreviouslyPracticedTexts);

				if (selectedTexts.Count < numberOfRestTextsForPractice)
				{
					// Taking first texts that were not practiced before.
					var selectedPreviouslyUnpracticedTexts = previouslyUnpracticedTexts
						.Take(numberOfRestTextsForPractice - selectedTexts.Count);

					selectedTexts.AddRange(selectedPreviouslyUnpracticedTexts);
				}
			}
			else
			{
				selectedTexts.AddRange(previouslyUnpracticedTexts);
				selectedTexts.AddRange(selectedPreviouslyPracticedTexts);
			}

			return selectedTexts
				.Take(numberOfRestTextsForPractice)
				.Randomize()
				.ToList();
		}
	}
}
