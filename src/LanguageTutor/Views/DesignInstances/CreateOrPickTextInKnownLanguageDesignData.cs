using System.Collections.ObjectModel;
using System.Linq;
using LanguageTutor.Models;
using LanguageTutor.ViewModels;

namespace LanguageTutor.Views.DesignInstances
{
	internal sealed class CreateOrPickTextInKnownLanguageDesignData : BasicCreateOrPickTextDesignData
	{
		public override Language Language => DesignData.KnownLanguage;

		public override bool RequireSpellCheck => false;

		public override bool CreatePronunciationRecord => false;

		public override string Text { get; set; } = "автомобиль";

		public override bool TextWasSpellChecked => false;

		public override bool TextIsFilled => true;

		public override string Note { get; set; } = "машина";

		public override ObservableCollection<LanguageTextViewModel> ExistingTexts { get; } = new(DesignData.TextsInKnownLanguage.Select(x => new LanguageTextViewModel(x)));
	}
}
