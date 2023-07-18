using System.Collections.ObjectModel;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels;

namespace VocabularyCoach.Views.DesignInstances
{
	internal sealed class EditTextInStudiedLanguageDesignData : EditLanguageTextDesignData
	{
		public override Language Language => DesignData.StudiedLanguage;

		public override bool RequireSpellCheck => true;

		public override bool CreatePronunciationRecord => true;

		public override ObservableCollection<LanguageTextViewModel> ExistingTexts { get; } = new()
		{
			new LanguageTextViewModel(
				new LanguageText
				{
					Id = new ItemId("1"),
					Language = DesignData.StudiedLanguage,
					Text = "dziękuję",
				}),

			new LanguageTextViewModel(
				new LanguageText
				{
					Id = new ItemId("2"),
					Language = DesignData.StudiedLanguage,
					Text = "proszę",
				}),
		};

		public override string Text { get; set; } = "samochód";

		public override bool TextWasSpellChecked => false;

		public override string Note { get; set; } = "auto";
	}
}
