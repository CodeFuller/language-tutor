using System.Collections.ObjectModel;
using System.Linq;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels;

namespace VocabularyCoach.Views.DesignInstances
{
	internal sealed class EditTextInKnownLanguageDesignData : EditLanguageTextDesignData
	{
		public override Language Language => DesignData.KnownLanguage;

		public override bool RequireSpellCheck => false;

		public override bool CreatePronunciationRecord => false;

		public override ObservableCollection<LanguageTextViewModel> ExistingTexts { get; } = new(DesignData.TextsInKnownLanguage.Select(x => new LanguageTextViewModel(x)));

		public override string Text { get; set; } = "автомобиль";

		public override bool TextWasSpellChecked => false;

		public override string Note { get; set; } = "машина";
	}
}
