using System.Collections.ObjectModel;
using System.Linq;
using LanguageTutor.Models;
using LanguageTutor.ViewModels;

namespace LanguageTutor.Views.DesignInstances
{
	internal sealed class CreateOrPickTextInStudiedLanguageDesignData : BasicCreateOrPickTextDesignData
	{
		public override Language Language => DesignData.StudiedLanguage;

		public override bool RequireSpellCheck => true;

		public override bool CreatePronunciationRecord => true;

		public override string Text { get; set; } = "samochód";

		public override bool TextWasSpellChecked => true;

		public override bool TextIsFilled => true;

		public override string Note { get; set; } = "auto";

		public override ObservableCollection<LanguageTextViewModel> ExistingTexts { get; } = new(DesignData.TextsInStudiedLanguage.Select(x => new LanguageTextViewModel(x)));
	}
}
