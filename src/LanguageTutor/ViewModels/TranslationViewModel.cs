using System;
using LanguageTutor.Models;

namespace LanguageTutor.ViewModels
{
	public class TranslationViewModel
	{
		public Translation Translation { get; }

		public LanguageTextViewModel LanguageText1 { get; }

		public LanguageTextViewModel LanguageText2 { get; }

		public TranslationViewModel(Translation translation)
		{
			Translation = translation ?? throw new ArgumentNullException(nameof(translation));
			LanguageText1 = new LanguageTextViewModel(translation.Text1);
			LanguageText2 = new LanguageTextViewModel(translation.Text2);
		}

		public override string ToString()
		{
			return $"{LanguageText1} - {LanguageText2}";
		}
	}
}
