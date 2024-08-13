using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.ViewModels.Exercises;

namespace LanguageTutor.Views.DesignInstances.Exercises
{
	internal class TranslateTextExerciseDesignData : ITranslateTextExerciseViewModel
	{
		public TranslateTextExercise Exercise { get; } = new([])
		{
			TextInStudiedLanguage = new()
			{
				Id = new ItemId("1"),
				Language = DesignData.StudiedLanguage,
				Text = "zamek",
			},

			OtherSynonymsInStudiedLanguage = new[]
			{
				new LanguageText
				{
					Id = new ItemId("2"),
					Language = DesignData.StudiedLanguage,
					Text = "warownia",
				},

				new LanguageText
				{
					Id = new ItemId("3"),
					Language = DesignData.StudiedLanguage,
					Text = "twierdza",
				},
			},

			SynonymsInKnownLanguage = new[]
			{
				new LanguageText
				{
					Id = new ItemId("4"),
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

		public bool ExerciseWasChecked => true;

		public bool ExerciseWasPerformedCorrectly => false;

		public bool ExerciseWasPerformedIncorrectly => true;

		public ICommand NextStepCommand => null;

		public ICommand PlayPronunciationRecordCommand => null;

		public Task<BasicExerciseResult> CheckExercise(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
