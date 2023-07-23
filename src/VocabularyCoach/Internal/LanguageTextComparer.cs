using System;
using System.Collections.Generic;
using VocabularyCoach.Models;

namespace VocabularyCoach.Internal
{
	internal sealed class LanguageTextComparer : IComparer<LanguageText>
	{
		public int Compare(LanguageText x, LanguageText y)
		{
			if (ReferenceEquals(x, y))
			{
				return 0;
			}

			if (y is null)
			{
				return 1;
			}

			if (x is null)
			{
				return -1;
			}

			var textComparison = String.Compare(x.Text, y.Text, LanguageTextComparison.IgnoreCase);
			if (textComparison != 0)
			{
				return textComparison;
			}

			return String.Compare(x.Note, y.Note, LanguageTextComparison.IgnoreCase);
		}
	}
}
