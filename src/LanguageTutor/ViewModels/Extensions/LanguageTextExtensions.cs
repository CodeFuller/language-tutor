using System;
using LanguageTutor.Models;

namespace LanguageTutor.ViewModels.Extensions
{
	internal static class LanguageTextExtensions
	{
		public static string GetTextWithNote(this LanguageText languageText)
		{
			return String.IsNullOrEmpty(languageText.Note) ? languageText.Text : $"{languageText.Text} ({languageText.Note})";
		}
	}
}
