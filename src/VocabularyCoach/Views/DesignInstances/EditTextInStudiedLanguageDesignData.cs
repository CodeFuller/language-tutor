using System.Collections.ObjectModel;
using System.Linq;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels;

namespace VocabularyCoach.Views.DesignInstances
{
	internal sealed class EditTextInStudiedLanguageDesignData : EditLanguageTextDesignData
	{
		public override Language Language => DesignData.StudiedLanguage;

		public override bool RequireSpellCheck => true;

		public override bool CreatePronunciationRecord => true;

		public override ObservableCollection<LanguageTextViewModel> ExistingTexts { get; } = new(DesignData.TextsInStudiedLanguage.Select(x => new LanguageTextViewModel(x)));

		public override string Text { get; set; } = "samochód";

		public override bool TextWasSpellChecked => false;

		public override string Note { get; set; } = "auto";
	}
}
