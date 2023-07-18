using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels
{
	public class TranslationViewModel
	{
		private readonly LanguageTextViewModel languageText1;

		private readonly LanguageTextViewModel languageText2;

		public TranslationViewModel(Translation translation)
		{
			languageText1 = new LanguageTextViewModel(translation.Text1);
			languageText2 = new LanguageTextViewModel(translation.Text2);
		}

		public override string ToString()
		{
			return $"{languageText1} - {languageText2}";
		}
	}
}
