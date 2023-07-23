using System;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Extensions
{
	internal static class LanguageTextExtensions
	{
		public static string GetTextWithNote(this LanguageText languageText)
		{
			return String.IsNullOrEmpty(languageText.Note) ? languageText.Text : $"{languageText.Text} ({languageText.Note})";
		}
	}
}
