using System.Collections.Generic;
using System.Linq;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.UnitTests.Helpers
{
	internal static class StudiedTextsExtensions
	{
		public static StudiedText Get(this IEnumerable<StudiedText> studiedTexts, string itemId)
		{
			return studiedTexts.Single(x => x.TextInStudiedLanguage.Id == new ItemId(itemId));
		}
	}
}
