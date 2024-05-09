using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models;
using LanguageTutor.ViewModels.Extensions;

namespace LanguageTutor.ViewModels
{
	public class ProblematicTextViewModel
	{
		public LanguageText TextInStudiedLanguage { get; }

		public string TranslationsInKnownLanguage { get; }

		public IReadOnlyCollection<ProblematicTextCheckResultViewModel> CheckResults { get; }

		public ProblematicTextViewModel(StudiedText studiedText)
		{
			TextInStudiedLanguage = studiedText.TextInStudiedLanguage;
			TranslationsInKnownLanguage = studiedText.GetTranslationsInKnownLanguage();

			CheckResults = studiedText.CheckResults
				.OrderBy(x => x.DateTime)
				.Select(x => new ProblematicTextCheckResultViewModel(x)).ToList();
		}
	}
}
