using System;
using System.Collections.Generic;
using System.Linq;

namespace VocabularyCoach.Services.Extensions
{
	internal static class EnumerableExtensions
	{
		public static IEnumerable<T> Randomize<T>(this IEnumerable<T> collection)
		{
			var rnd = new Random();
			return collection.OrderBy(a => Guid.NewGuid()).ToList();
		}
	}
}
