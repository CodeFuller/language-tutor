using FluentAssertions;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Internal;
using LanguageTutor.Services.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace LanguageTutor.Services.UnitTests.Internal
{
	[TestClass]
	public class TranslateTextExerciseFactoryTests
	{
		[TestMethod]
		public void CreateTranslateTextExercises_ForTranslationsWithoutSynonyms_DoesNotGroupTranslations()
		{
			// Arrange

			var exercisesData = new TranslateTextExerciseData[]
			{
				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextsInKnownLanguage = ["Text in known language #1".ToLanguageText()],
					ExerciseResults = [],
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					TextsInKnownLanguage = ["Text in known language #2".ToLanguageText()],
					ExerciseResults = [],
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TranslateTextExerciseFactory>();

			// Act

			var result = target.CreateTranslateTextExercises(exercisesData);

			// Assert

			var expectedResult = new TranslateTextExercise[]
			{
				new([])
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = [],
					SynonymsInKnownLanguage = ["Text in known language #1".ToLanguageText()],
				},

				new([])
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = [],
					SynonymsInKnownLanguage = ["Text in known language #2".ToLanguageText()],
				},
			};

			result.Should().BeEquivalentTo(expectedResult, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void CreateTranslateTextExercises_ForSynonymsIsStudiedLanguageWithSingleTranslation_GroupsSynonymsCorrectly()
		{
			// Arrange

			var exercisesData = new TranslateTextExerciseData[]
			{
				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextsInKnownLanguage = ["Text in known language #1".ToLanguageText()],
					ExerciseResults = [],
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					TextsInKnownLanguage = ["Text in known language #1".ToLanguageText()],
					ExerciseResults = [],
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #3".ToLanguageText(),
					TextsInKnownLanguage = ["Text in known language #1".ToLanguageText()],
					ExerciseResults = [],
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TranslateTextExerciseFactory>();

			// Act

			var result = target.CreateTranslateTextExercises(exercisesData);

			// Assert

			var expectedResult = new TranslateTextExercise[]
			{
				new([])
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					OtherSynonymsInStudiedLanguage =
					[
						"Text in studied language #2".ToLanguageText(),
						"Text in studied language #3".ToLanguageText(),
					],
					SynonymsInKnownLanguage = ["Text in known language #1".ToLanguageText()],
				},

				new([])
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					OtherSynonymsInStudiedLanguage =
					[
						"Text in studied language #1".ToLanguageText(),
						"Text in studied language #3".ToLanguageText(),
					],
					SynonymsInKnownLanguage = ["Text in known language #1".ToLanguageText()],
				},

				new([])
				{
					TextInStudiedLanguage = "Text in studied language #3".ToLanguageText(),
					OtherSynonymsInStudiedLanguage =
					[
						"Text in studied language #1".ToLanguageText(),
						"Text in studied language #2".ToLanguageText(),
					],
					SynonymsInKnownLanguage = ["Text in known language #1".ToLanguageText()],
				},
			};

			result.Should().BeEquivalentTo(expectedResult, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void CreateTranslateTextExercises_ForSynonymsIsStudiedLanguageWithMultipleTranslations_GroupsSynonymsCorrectly()
		{
			// Arrange

			var exercisesData = new TranslateTextExerciseData[]
			{
				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextsInKnownLanguage = [
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
					],
					ExerciseResults = [],
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					TextsInKnownLanguage = [
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
					],
					ExerciseResults = [],
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TranslateTextExerciseFactory>();

			// Act

			var result = target.CreateTranslateTextExercises(exercisesData);

			// Assert

			var expectedResult = new TranslateTextExercise[]
			{
				new([])
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					OtherSynonymsInStudiedLanguage =
					[
						"Text in studied language #2".ToLanguageText(),
					],
					SynonymsInKnownLanguage =
					[
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
					],
				},

				new([])
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					OtherSynonymsInStudiedLanguage =
					[
						"Text in studied language #1".ToLanguageText(),
					],
					SynonymsInKnownLanguage =
					[
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
					],
				},
			};

			result.Should().BeEquivalentTo(expectedResult, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void CreateTranslateTextExercises_ForSynonymsIsStudiedLanguageWithPartialTranslationsMatching_GroupsSynonymsCorrectly()
		{
			// Arrange

			var exercisesData = new TranslateTextExerciseData[]
			{
				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextsInKnownLanguage = [
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
					],
					ExerciseResults = [],
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					TextsInKnownLanguage = ["Text in known language #1".ToLanguageText()],
					ExerciseResults = [],
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TranslateTextExerciseFactory>();

			// Act

			var result = target.CreateTranslateTextExercises(exercisesData);

			// Assert

			var expectedResult = new TranslateTextExercise[]
			{
				new([])
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = [],
					SynonymsInKnownLanguage =
					[
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
					],
				},

				new([])
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					OtherSynonymsInStudiedLanguage =
					[
						"Text in studied language #1".ToLanguageText(),
					],
					SynonymsInKnownLanguage =
					[
						"Text in known language #1".ToLanguageText(),
					],
				},
			};

			result.Should().BeEquivalentTo(expectedResult, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void CreateTranslateTextExercises_ForSynonymsIsKnownLanguage_GroupsSynonymsCorrectly()
		{
			// Arrange

			var exercisesData = new TranslateTextExerciseData[]
			{
				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextsInKnownLanguage = [
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
						"Text in known language #3".ToLanguageText(),
					],
					ExerciseResults = [],
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<TranslateTextExerciseFactory>();

			// Act

			var result = target.CreateTranslateTextExercises(exercisesData);

			// Assert

			var expectedResult = new TranslateTextExercise[]
			{
				new([])
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = [],
					SynonymsInKnownLanguage =
					[
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
						"Text in known language #3".ToLanguageText(),
					],
				},
			};

			result.Should().BeEquivalentTo(expectedResult, x => x.WithoutStrictOrdering());
		}
	}
}
