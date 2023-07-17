using System;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels
{
	public sealed class LanguageTextViewModel
	{
		public LanguageText LanguageText { get; }

		public string Text => LanguageText.Text;

		public string Note => LanguageText.Note;

		public string TextWithNote => String.IsNullOrEmpty(LanguageText.Note) ? LanguageText.Text : $"{LanguageText.Text} ({LanguageText.Note})";

		public LanguageTextViewModel(LanguageText languageText)
		{
			LanguageText = languageText ?? throw new ArgumentNullException(nameof(languageText));
		}

		public override string ToString()
		{
			return TextWithNote;
		}
	}
}
