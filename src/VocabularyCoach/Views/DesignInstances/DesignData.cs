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
	}
}
