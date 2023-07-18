using System;
using System.Collections.ObjectModel;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Extensions
{
	internal static class ObservableCollectionExtensions
	{
		public static void AddToSortedCollection<T>(this ObservableCollection<T> collection, T item)
		{
			for (var i = 0; i < collection.Count + 1; ++i)
			{
				if (i == collection.Count || String.Compare(item.ToString(), collection[i].ToString(), LanguageTextComparison.IgnoreCase) < 0)
				{
					collection.Insert(i, item);
					break;
				}
			}
		}
	}
}
