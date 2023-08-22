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

#pragma warning disable CA5394 // Do not use insecure randomness
			return collection.OrderBy(_ => rnd.Next()).ToList();
#pragma warning restore CA5394 // Do not use insecure randomness
		}
	}
}
