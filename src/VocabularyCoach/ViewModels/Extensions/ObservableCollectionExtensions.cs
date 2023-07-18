using System;
using System.Collections.ObjectModel;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Extensions
{
	internal static class ObservableCollectionExtensions
	{
		public static void AddTextToSortedCollection(this ObservableCollection<LanguageTextViewModel> textsCollection, LanguageText languageText)
		{
			for (var i = 0; i < textsCollection.Count + 1; ++i)
			{
				if (i == textsCollection.Count || String.Compare(languageText.Text, textsCollection[i].Text, LanguageTextComparison.IgnoreCase) < 0)
				{
					textsCollection.Insert(i, new LanguageTextViewModel(languageText));
					break;
				}
			}
		}
	}
}
