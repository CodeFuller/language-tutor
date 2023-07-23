using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.Views.DesignInstances
{
	internal sealed class PracticeVocabularyDesignData : IPracticeVocabularyViewModel
	{
		public int NumberOfTextsForCheck => 100;

		public int CurrentTextForCheckNumber => 25;

		public string ProgressInfo => "25 / 100";

		public StudiedText CurrentTextForCheck { get; } = new(Enumerable.Empty<CheckResult>())
		{
			TextInStudiedLanguage = new()
			{
				Id = new ItemId("1"),
				Language = DesignData.StudiedLanguage,
				Text = "zamek",
			},

			SynonymsInKnownLanguage = new[]
			{
				new LanguageText
				{
					Id = new ItemId("2"),
					Language = DesignData.KnownLanguage,
					Text = "замок",
					Note = "строение",
				},
			},
		};

		public string DisplayedTextInKnownLanguage => "замок (строение)";

		public string HintForOtherSynonyms => "synonyms: warownia, twierdza";

		public bool PronunciationRecordExists => true;

		public bool TypedTextIsFocused => false;

		public string TypedText { get; set; } = "zamok";

		public bool CheckResultIsShown => true;

		public bool TextIsTypedCorrectly => false;

		public bool TextIsTypedIncorrectly => true;

		public bool CanSwitchToNextText => true;

		public ICommand CheckTypedTextCommand => null;

		public ICommand SwitchToNextTextCommand => null;

		public ICommand CheckOrSwitchToNextTextCommand => null;

		public ICommand PlayPronunciationRecordCommand => null;

		public ICommand FinishPracticeCommand => null;

		public Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
