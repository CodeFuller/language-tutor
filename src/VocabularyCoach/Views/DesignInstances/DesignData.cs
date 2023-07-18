using System.Collections.Generic;
using System.Linq;
using VocabularyCoach.Models;

namespace VocabularyCoach.Views.DesignInstances
{
	internal static class DesignData
	{
		public static Language StudiedLanguage { get; } = new()
		{
			Id = new ItemId("2"),
			Name = "Polish",
		};

		public static Language KnownLanguage { get; } = new()
		{
			Id = new ItemId("1"),
			Name = "Russian",
		};

		public static IReadOnlyCollection<LanguageText> TextsInStudiedLanguage { get; } = new[]
		{
			new LanguageText
			{
				Id = new ItemId("1"),
				Language = DesignData.StudiedLanguage,
				Text = "dziękuję",
			},

			new LanguageText
			{
				Id = new ItemId("2"),
				Language = DesignData.StudiedLanguage,
				Text = "proszę",
			},
		};

		public static IReadOnlyCollection<LanguageText> TextsInKnownLanguage { get; } = new[]
		{
			new LanguageText
			{
				Id = new ItemId("3"),
				Language = DesignData.KnownLanguage,
				Text = "спасибо",
			},

			new LanguageText
			{
				Id = new ItemId("4"),
				Language = DesignData.KnownLanguage,
				Text = "пожалуйста",
				Note = "ответ на спасибо",
			},
		};

		public static IReadOnlyCollection<Translation> Translations { get; } = TextsInStudiedLanguage.Zip(TextsInKnownLanguage)
			.Select(x => new Translation
			{
				Text1 = x.First,
				Text2 = x.Second,
			})
			.ToList();
	}
}
