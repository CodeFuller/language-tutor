using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.ViewModels;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.Views.DesignInstances
{
	internal class ProblematicTextsDesignData : IProblematicTextsViewModel
	{
		public ObservableCollection<ProblematicTextViewModel> ProblematicTexts { get; } = new()
		{
			new ProblematicTextViewModel(new StudiedText(new CheckResult[]
			{
				new()
				{
					CheckResultType = CheckResultType.Skipped,
					DateTime = new DateTimeOffset(2023, 08, 17, 08, 16, 41, TimeSpan.FromHours(2)),
				},

				new()
				{
					CheckResultType = CheckResultType.Misspelled,
					DateTime = new DateTimeOffset(2023, 08, 19, 11, 01, 09, TimeSpan.FromHours(2)),
					TypedText = "źmęczony",
				},

				new()
				{
					CheckResultType = CheckResultType.Ok,
					DateTime = new DateTimeOffset(2023, 08, 20, 10, 48, 05, TimeSpan.FromHours(2)),
				},
			})
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "zmęczony",
				},

				SynonymsInKnownLanguage = new[]
				{
					new LanguageText { Text = "уставший" },
					new LanguageText { Text = "усталый" },
				},
			}),

			new ProblematicTextViewModel(new StudiedText(new CheckResult[]
			{
				new() { CheckResultType = CheckResultType.Skipped },
				new() { CheckResultType = CheckResultType.Skipped },
				new() { CheckResultType = CheckResultType.Misspelled },
				new() { CheckResultType = CheckResultType.Misspelled },
				new() { CheckResultType = CheckResultType.Misspelled },
			})
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "pewny",
				},

				SynonymsInKnownLanguage = new[]
				{
					new LanguageText { Text = "уверенный" },
				},
			}),
		};

		public ProblematicTextViewModel SelectedText { get; set; }

		public ICommand GoToStartPageCommand => null;

		public ProblematicTextsDesignData()
		{
			SelectedText = ProblematicTexts.First();
		}

		public Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
