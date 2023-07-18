using System.Collections.ObjectModel;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels;

namespace VocabularyCoach.Views.DesignInstances
{
	internal sealed class EditTextInKnownLanguageDesignData : EditLanguageTextDesignData
	{
		public override Language Language => DesignData.KnownLanguage;

		public override bool RequireSpellCheck => false;

		public override bool CreatePronunciationRecord => false;

		public override ObservableCollection<LanguageTextViewModel> ExistingTexts { get; } = new()
		{
			new LanguageTextViewModel(
				new LanguageText
				{
					Id = new ItemId("3"),
					Language = DesignData.KnownLanguage,
					Text = "спасибо",
				}),

			new LanguageTextViewModel(
				new LanguageText
				{
					Id = new ItemId("4"),
					Language = DesignData.KnownLanguage,
					Text = "пожалуйста",
					Note = "ответ на спасибо",
				}),
		};

		public override string Text { get; set; } = "автомобиль";

		public override bool TextWasSpellChecked => false;

		public override string Note { get; set; } = "машина";
	}
}
