using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using FluentAssertions;
using LanguageTutor.Models;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.UnitTests.Helpers;
using LanguageTutor.ViewModels;
using LanguageTutor.ViewModels.ContextMenu;
using LanguageTutor.ViewModels.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LanguageTutor.UnitTests.ViewModels
{
	[TestClass]
	public class EditDictionaryViewModelTests
	{
		private sealed class TestTargetDependencies
		{
			public Mock<IDictionaryService> DictionaryServiceMock { get; } = new();

			public Mock<ICreateOrPickTextViewModel> CreateOrPickTextInStudiedLanguageViewModelMock { get; } = new();

			public Mock<ICreateOrPickTextViewModel> CreateOrPickTextInKnownLanguageViewModelMock { get; } = new();

			public Mock<IEditExistingTextViewModel> EditExistingTextInStudiedLanguageViewModelMock { get; } = new();

			public Mock<IEditExistingTextViewModel> EditExistingTextInKnownLanguageViewModelMock { get; } = new();

			public ICreateOrPickTextViewModel CreateOrPickTextInStudiedLanguageViewModel => CreateOrPickTextInStudiedLanguageViewModelMock.Object;

			public ICreateOrPickTextViewModel CreateOrPickTextInKnownLanguageViewModel => CreateOrPickTextInKnownLanguageViewModelMock.Object;

			public IEditExistingTextViewModel EditExistingTextInStudiedLanguageViewModel => EditExistingTextInStudiedLanguageViewModelMock.Object;

			public IEditExistingTextViewModel EditExistingTextInKnownLanguageViewModel => EditExistingTextInKnownLanguageViewModelMock.Object;

			public void ClearInvocations()
			{
				DictionaryServiceMock.Invocations.Clear();
				CreateOrPickTextInStudiedLanguageViewModelMock.Invocations.Clear();
				CreateOrPickTextInKnownLanguageViewModelMock.Invocations.Clear();
				EditExistingTextInStudiedLanguageViewModelMock.Invocations.Clear();
				EditExistingTextInKnownLanguageViewModelMock.Invocations.Clear();
			}
		}

		private Language TestStudiedLanguage { get; } = new()
		{
			Id = new ItemId("Test Studied Language Id"),
			Name = "Test Studied Language",
		};

		private Language TestKnownLanguage { get; } = new()
		{
			Id = new ItemId("Test Known Language Id"),
			Name = "Test Known Language",
		};

		private IReadOnlyList<LanguageText> TestLanguageTextsInStudiedLanguage => new[]
		{
			new LanguageText
			{
				Id = new ItemId("test text in studied language id 1"),
				Language = TestStudiedLanguage,
				Text = "test text in studied language 1",
				Note = "test note 1",
			},

			new LanguageText
			{
				Id = new ItemId("test text in studied language id 2"),
				Language = TestStudiedLanguage,
				Text = "test text in studied language 2",
				Note = "test note 2",
			},
		};

		private IReadOnlyList<LanguageText> TestLanguageTextsInKnownLanguage => new[]
		{
			new LanguageText
			{
				Id = new ItemId("test text in known language id 1"),
				Language = TestKnownLanguage,
				Text = "test text in known language 1",
				Note = "test note 1",
			},

			new LanguageText
			{
				Id = new ItemId("test text in known language id 2"),
				Language = TestKnownLanguage,
				Text = "test text in known language 2",
				Note = "test note 2",
			},
		};

		private IReadOnlyList<Translation> TestTranslations => new[]
		{
			new Translation
			{
				Text1 = TestLanguageTextsInStudiedLanguage[0],
				Text2 = TestLanguageTextsInKnownLanguage[0],
			},

			new Translation
			{
				Text1 = TestLanguageTextsInStudiedLanguage[1],
				Text2 = TestLanguageTextsInKnownLanguage[0],
			},

			new Translation
			{
				Text1 = TestLanguageTextsInStudiedLanguage[1],
				Text2 = TestLanguageTextsInKnownLanguage[1],
			},
		};

		private IReadOnlyList<TranslationViewModel> TestTranslationViewModels => TestTranslations.Select(x => new TranslationViewModel(x)).ToList();

		private IReadOnlyList<Translation> TestNewTranslations => new[]
		{
			new Translation
			{
				Text1 = new LanguageText
				{
					Id = new ItemId("test text in studied language id 3"),
					Language = TestStudiedLanguage,
					Text = "test text in studied language 3",
					Note = "test note 3",
				},

				Text2 = new LanguageText
				{
					Id = new ItemId("test text in known language id 3"),
					Language = TestKnownLanguage,
					Text = "test text in known language 3",
					Note = "test note 3",
				},
			},
		};

		private IReadOnlyList<TranslationViewModel> TestNewTranslationViewModels => TestNewTranslations.Select(x => new TranslationViewModel(x)).ToList();

		[TestMethod]
		public void Constructor_IfSameInstanceIsPassedForCreateOrPickTextViewModels_Throws()
		{
			var createOrPickTextViewModel = Mock.Of<ICreateOrPickTextViewModel>();

			var call = () => new EditDictionaryViewModel(Mock.Of<IDictionaryService>(), Mock.Of<IMessenger>(),
				createOrPickTextViewModel, createOrPickTextViewModel,
				Mock.Of<IEditExistingTextViewModel>(), Mock.Of<IEditExistingTextViewModel>());

			call.Should().Throw<ArgumentException>().WithMessage("The same instance is injected for createOrPickTextInStudiedLanguageViewModel and createOrPickTextInKnownLanguageViewModel");
		}

		[TestMethod]
		public void Constructor_IfSameInstanceIsPassedForEditExistingTextViewModels_Throws()
		{
			var editExistingTextViewModel = Mock.Of<IEditExistingTextViewModel>();

			var call = () => new EditDictionaryViewModel(Mock.Of<IDictionaryService>(), Mock.Of<IMessenger>(),
				Mock.Of<ICreateOrPickTextViewModel>(), Mock.Of<ICreateOrPickTextViewModel>(),
				editExistingTextViewModel, editExistingTextViewModel);

			call.Should().Throw<ArgumentException>().WithMessage("The same instance is injected for editExistingTextInStudiedLanguageViewModel and editExistingTextInKnownLanguageViewModel");
		}

		[TestMethod]
		public async Task Load_ForFirstCall_InitializesViewModelCorrectly()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();
			var target = CreateTestTarget(dependencies);

			// Act

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Assert

			var expectedViewModelData = new EditDictionaryViewModelData
			{
				CurrentTextInStudiedLanguageViewModel = dependencies.CreateOrPickTextInStudiedLanguageViewModel,
				CurrentTextInKnownLanguageViewModel = dependencies.CreateOrPickTextInKnownLanguageViewModel,
				EditTextInStudiedLanguageIsEnabled = true,
				EditTextInKnownLanguageIsEnabled = true,
				TranslationFilter = String.Empty,
				FilteredTranslations = TestTranslationViewModels,
				SelectedTranslation = null,
			};

			target.Should().BeEquivalentTo(expectedViewModelData, EditDictionaryViewModelData.ExcludingCommands);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.Load(TestStudiedLanguage, true, true, It.IsAny<CancellationToken>()), Times.Once);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.Load(TestKnownLanguage, false, false, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task Load_ForSubsequentCall_InitializesViewModelCorrectly()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();
			var target = CreateTestTarget(dependencies);

			// Act

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Emulating edit of ViewModels data.
			target.TranslationFilter = "test text";
			target.SelectedTranslation = target.FilteredTranslations.First();

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Invocations.Clear();
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Invocations.Clear();

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Assert

			var expectedViewModelData = new EditDictionaryViewModelData
			{
				CurrentTextInStudiedLanguageViewModel = dependencies.CreateOrPickTextInStudiedLanguageViewModel,
				CurrentTextInKnownLanguageViewModel = dependencies.CreateOrPickTextInKnownLanguageViewModel,
				EditTextInStudiedLanguageIsEnabled = true,
				EditTextInKnownLanguageIsEnabled = true,
				TranslationFilter = String.Empty,
				FilteredTranslations = TestTranslationViewModels,
				SelectedTranslation = null,
			};

			target.Should().BeEquivalentTo(expectedViewModelData, EditDictionaryViewModelData.ExcludingCommands);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.Load(TestStudiedLanguage, true, true, It.IsAny<CancellationToken>()), Times.Once);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.Load(TestKnownLanguage, false, false, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task SaveChangesCommand_ForNewTranslationModeIfViewModelForTextInStudiedLanguageHasValidationErrors_ReturnsWithoutSavingChanges()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var validationIsEnabled = false;

			var createOrPickTextInStudiedLanguageViewModelMock = dependencies.CreateOrPickTextInStudiedLanguageViewModelMock;
			createOrPickTextInStudiedLanguageViewModelMock.SetupSet(x => x.ValidationIsEnabled = It.IsAny<bool>()).Callback((bool value) => validationIsEnabled = value);
			createOrPickTextInStudiedLanguageViewModelMock.Setup(x => x.HasErrors).Returns(() => validationIsEnabled);

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Act

			await target.SaveChangesCommand.ExecuteAsync(null);

			// Assert

			createOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
			dependencies.DictionaryServiceMock.Verify(x => x.AddTranslation(It.IsAny<LanguageText>(), It.IsAny<LanguageText>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task SaveChangesCommand_ForNewTranslationModeIfViewModelForTextInKnownLanguageHasValidationErrors_ReturnsWithoutSavingChanges()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var validationIsEnabled = false;

			var createOrPickTextInKnownLanguageViewModelMock = dependencies.CreateOrPickTextInKnownLanguageViewModelMock;
			createOrPickTextInKnownLanguageViewModelMock.SetupSet(x => x.ValidationIsEnabled = It.IsAny<bool>()).Callback((bool value) => validationIsEnabled = value);
			createOrPickTextInKnownLanguageViewModelMock.Setup(x => x.HasErrors).Returns(() => validationIsEnabled);

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Act

			await target.SaveChangesCommand.ExecuteAsync(null);

			// Assert

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
			createOrPickTextInKnownLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
			dependencies.DictionaryServiceMock.Verify(x => x.AddTranslation(It.IsAny<LanguageText>(), It.IsAny<LanguageText>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task SaveChangesCommand_ForNewTranslationModeWithNoValidationErrors_SavesChangesCorrectly()
		{
			// Arrange

			var newTextInStudiedLanguage = new LanguageText
			{
				Id = new ItemId("new text in studied language id"),
				Language = TestStudiedLanguage,
				Text = "new text in studied language",
			};

			var newTextInKnownLanguage = new LanguageText
			{
				Id = new ItemId("new text in known language id"),
				Language = TestKnownLanguage,
				Text = "new text in known language",
			};

			var newTranslation = new Translation
			{
				Text1 = newTextInStudiedLanguage,
				Text2 = newTextInKnownLanguage,
			};

			var dependencies = new TestTargetDependencies();

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Setup(x => x.SaveChanges(It.IsAny<CancellationToken>())).ReturnsAsync(newTextInStudiedLanguage);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Setup(x => x.SaveChanges(It.IsAny<CancellationToken>())).ReturnsAsync(newTextInKnownLanguage);
			dependencies.DictionaryServiceMock.Setup(x => x.AddTranslation(newTextInStudiedLanguage, newTextInKnownLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(newTranslation);

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Act

			await target.SaveChangesCommand.ExecuteAsync(null);

			// Assert

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
			dependencies.DictionaryServiceMock.Verify(x => x.AddTranslation(newTextInStudiedLanguage, newTextInKnownLanguage, It.IsAny<CancellationToken>()), Times.Once);

			var expectedTranslations = new[]
			{
				new TranslationViewModel(newTranslation),
				new TranslationViewModel(TestTranslations[0]),
				new TranslationViewModel(TestTranslations[1]),
				new TranslationViewModel(TestTranslations[2]),
			};

			target.FilteredTranslations.Should().BeEquivalentTo(expectedTranslations, x => x.WithStrictOrdering());
		}

		[TestMethod]
		public async Task SaveChangesCommand_ForNewTranslationMode_ClearsDataAfterSaving()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var newTranslation = new Translation
			{
				Text1 = new LanguageText(),
				Text2 = new LanguageText(),
			};

			dependencies.DictionaryServiceMock.Setup(x => x.AddTranslation(It.IsAny<LanguageText>(), It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(newTranslation);

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Invocations.Clear();
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Invocations.Clear();
			dependencies.DictionaryServiceMock.Invocations.Clear();

			// Act

			// Emulating edit of ViewModels data that should not be cleared.
			target.TranslationFilter = "test text";
			target.SelectedTranslation = target.FilteredTranslations.First();

			await target.SaveChangesCommand.ExecuteAsync(null);

			// Assert

			var expectedViewModelData = new EditDictionaryViewModelData
			{
				CurrentTextInStudiedLanguageViewModel = dependencies.CreateOrPickTextInStudiedLanguageViewModel,
				CurrentTextInKnownLanguageViewModel = dependencies.CreateOrPickTextInKnownLanguageViewModel,
				EditTextInStudiedLanguageIsEnabled = true,
				EditTextInKnownLanguageIsEnabled = true,
				TranslationFilter = "test text",
				FilteredTranslations = TestTranslationViewModels,
				SelectedTranslation = target.FilteredTranslations.First(),
			};

			target.Should().BeEquivalentTo(expectedViewModelData, EditDictionaryViewModelData.ExcludingCommands);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.ClearFilledData(), Times.Once);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.ClearFilledData(), Times.Once);
			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.Load(It.IsAny<Language>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.Load(It.IsAny<Language>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);

			dependencies.DictionaryServiceMock.Verify(x => x.GetTranslations(It.IsAny<Language>(), It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.VerifySet(x => x.TextIsFocused = true, Times.Once);
		}

		[TestMethod]
		public async Task SaveChangesCommand_ForEditTextInStudiedLanguageModeIfThereAreValidationErrors_ReturnsWithoutSavingChanges()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var validationIsEnabled = false;

			var editExistingTextInStudiedLanguageViewModelMock = dependencies.EditExistingTextInStudiedLanguageViewModelMock;
			editExistingTextInStudiedLanguageViewModelMock.SetupSet(x => x.ValidationIsEnabled = It.IsAny<bool>()).Callback((bool value) => validationIsEnabled = value);
			editExistingTextInStudiedLanguageViewModelMock.Setup(x => x.HasErrors).Returns(() => validationIsEnabled);

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);
			await SwitchToEditTextInStudiedLanguageMode(target, TestTranslationViewModels[0]);

			// Act

			await target.SaveChangesCommand.ExecuteAsync(null);

			// Assert

			editExistingTextInStudiedLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task SaveChangesCommand_ForEditTextInKnownLanguageModeIfThereAreValidationErrors_ReturnsWithoutSavingChanges()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var validationIsEnabled = false;

			var editExistingTextInKnownLanguageViewModelMock = dependencies.EditExistingTextInKnownLanguageViewModelMock;
			editExistingTextInKnownLanguageViewModelMock.SetupSet(x => x.ValidationIsEnabled = It.IsAny<bool>()).Callback((bool value) => validationIsEnabled = value);
			editExistingTextInKnownLanguageViewModelMock.Setup(x => x.HasErrors).Returns(() => validationIsEnabled);

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);
			await SwitchToEditTextInKnownLanguageMode(target, TestTranslationViewModels[0]);

			// Act

			await target.SaveChangesCommand.ExecuteAsync(null);

			// Assert

			editExistingTextInKnownLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task SaveChangesCommand_ForEditTextInStudiedLanguageModeWithNoValidationErrors_SavesChangesCorrectly()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);
			await SwitchToEditTextInStudiedLanguageMode(target, TestTranslationViewModels[0]);

			// Act

			await target.SaveChangesCommand.ExecuteAsync(null);

			// Assert

			dependencies.EditExistingTextInStudiedLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
			dependencies.EditExistingTextInKnownLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task SaveChangesCommand_ForEditTextInKnownLanguageModeWithNoValidationErrors_SavesChangesCorrectly()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);
			await SwitchToEditTextInKnownLanguageMode(target, TestTranslationViewModels[0]);

			// Act

			await target.SaveChangesCommand.ExecuteAsync(null);

			// Assert

			dependencies.EditExistingTextInStudiedLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
			dependencies.EditExistingTextInKnownLanguageViewModelMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task SaveChangesCommand_ForEditTextInStudiedLanguageModeWithNoValidationErrors_ReloadsDataAfterSaving()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);
			await SwitchToEditTextInStudiedLanguageMode(target, TestTranslationViewModels[0]);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Invocations.Clear();
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Invocations.Clear();

			// Stubbing new translations that should be reloaded after save.
			dependencies.DictionaryServiceMock.Setup(x => x.GetTranslations(TestStudiedLanguage, TestKnownLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestNewTranslations);

			// Act

			// Emulating edit of ViewModels data that should not be cleared.
			target.TranslationFilter = "test text";
			target.SelectedTranslation = target.FilteredTranslations.First();

			await target.SaveChangesCommand.ExecuteAsync(null);

			// Assert

			VerifyDataReloading(target, dependencies, TestNewTranslationViewModels);
		}

		[TestMethod]
		public async Task SaveChangesCommand_ForEditTextInKnownLanguageModeWithNoValidationErrors_ReloadsDataAfterSaving()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);
			await SwitchToEditTextInKnownLanguageMode(target, TestTranslationViewModels[0]);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Invocations.Clear();
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Invocations.Clear();

			// Stubbing new translations that should be reloaded after save.
			dependencies.DictionaryServiceMock.Setup(x => x.GetTranslations(TestStudiedLanguage, TestKnownLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestNewTranslations);

			// Act

			// Emulating edit of ViewModels data that should not be cleared.
			target.TranslationFilter = "test text";
			target.SelectedTranslation = target.FilteredTranslations.First();

			await target.SaveChangesCommand.ExecuteAsync(null);

			// Assert

			VerifyDataReloading(target, dependencies, TestNewTranslationViewModels);
		}

		[TestMethod]
		public async Task ClearChangesCommand_ForNewTranslationMode_ClearsChangesCorrectly()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Invocations.Clear();
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Invocations.Clear();
			dependencies.DictionaryServiceMock.Invocations.Clear();

			// Act

			// Emulating edit of ViewModels data that should not be cleared.
			target.TranslationFilter = "test text";
			target.SelectedTranslation = target.FilteredTranslations.First();

			target.ClearChangesCommand.Execute(null);

			// Assert

			var expectedViewModelData = new EditDictionaryViewModelData
			{
				CurrentTextInStudiedLanguageViewModel = dependencies.CreateOrPickTextInStudiedLanguageViewModel,
				CurrentTextInKnownLanguageViewModel = dependencies.CreateOrPickTextInKnownLanguageViewModel,
				EditTextInStudiedLanguageIsEnabled = true,
				EditTextInKnownLanguageIsEnabled = true,
				TranslationFilter = "test text",
				FilteredTranslations = TestTranslationViewModels,
				SelectedTranslation = target.FilteredTranslations.First(),
			};

			target.Should().BeEquivalentTo(expectedViewModelData, EditDictionaryViewModelData.ExcludingCommands);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.ClearFilledData(), Times.Once);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.ClearFilledData(), Times.Once);
			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.Load(It.IsAny<Language>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.Load(It.IsAny<Language>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);

			dependencies.DictionaryServiceMock.Verify(x => x.GetTranslations(It.IsAny<Language>(), It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.VerifySet(x => x.TextIsFocused = true, Times.Once);
		}

		[TestMethod]
		public async Task ClearChangesCommand_ForEditTextInStudiedLanguageMode_ClearsChangesCorrectly()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);
			await SwitchToEditTextInStudiedLanguageMode(target, TestTranslationViewModels[0]);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Invocations.Clear();
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Invocations.Clear();
			dependencies.DictionaryServiceMock.Invocations.Clear();

			// Act

			// Emulating edit of ViewModels data that should not be cleared.
			target.TranslationFilter = "test text";
			target.SelectedTranslation = target.FilteredTranslations.First();

			target.ClearChangesCommand.Execute(null);

			// Assert

			var expectedViewModelData = new EditDictionaryViewModelData
			{
				CurrentTextInStudiedLanguageViewModel = dependencies.CreateOrPickTextInStudiedLanguageViewModel,
				CurrentTextInKnownLanguageViewModel = dependencies.CreateOrPickTextInKnownLanguageViewModel,
				EditTextInStudiedLanguageIsEnabled = true,
				EditTextInKnownLanguageIsEnabled = true,
				TranslationFilter = "test text",
				FilteredTranslations = TestTranslationViewModels,
				SelectedTranslation = target.FilteredTranslations.First(),
			};

			target.Should().BeEquivalentTo(expectedViewModelData, EditDictionaryViewModelData.ExcludingCommands);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.ClearFilledData(), Times.Once);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.ClearFilledData(), Times.Once);
			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.Load(It.IsAny<Language>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.Load(It.IsAny<Language>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);

			dependencies.DictionaryServiceMock.Verify(x => x.GetTranslations(It.IsAny<Language>(), It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.VerifySet(x => x.TextIsFocused = true, Times.Once);
		}

		[TestMethod]
		public async Task GetContextMenuItemsForSelectedTranslation_IfNoTranslationIsSelected_ReturnsEmptyCollection()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Act

			var menuItems = target.GetContextMenuItemsForSelectedTranslation();

			// Assert

			menuItems.Should().BeEmpty();
		}

		[TestMethod]
		public async Task GetContextMenuItemsForSelectedTranslation_IfSomeTranslationIsSelected_ReturnsCorrectMenuItems()
		{
			// Arrange

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Act

			target.SelectedTranslation = TestTranslationViewModels[1];
			var menuItems = target.GetContextMenuItemsForSelectedTranslation();

			// Assert

			var expectedMenuItems = new[]
			{
				new ContextMenuItem { Header = "Edit Text 'test text in studied language 2 (test note 2)'" },
				new ContextMenuItem { Header = "Edit Text 'test text in known language 1 (test note 1)'" },
				new ContextMenuItem { Header = "Delete Translation" },
				new ContextMenuItem { Header = "Delete Text 'test text in studied language 2 (test note 2)' and 2 translation(s)" },
				new ContextMenuItem { Header = "Delete Text 'test text in known language 1 (test note 1)' and 2 translation(s)" },
				new ContextMenuItem { Header = "Delete Both Texts and 3 translation(s)" },
			};

			menuItems.Should().BeEquivalentTo(expectedMenuItems, x => x.WithStrictOrdering().Excluding(y => y.Command));
		}

		[TestMethod]
		public async Task EditLanguageTextInStudiedLanguage_SwitchesViewModelsCorrectly()
		{
			// Arrange

			var testTranslationViewModel = TestTranslationViewModels[1];

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			dependencies.ClearInvocations();

			// Act

			// Emulating edit of ViewModels data that should not be cleared.
			target.TranslationFilter = "test text";

			await SwitchToEditTextInStudiedLanguageMode(target, testTranslationViewModel);

			// Assert

			var expectedViewModelData = new EditDictionaryViewModelData
			{
				CurrentTextInStudiedLanguageViewModel = dependencies.EditExistingTextInStudiedLanguageViewModel,
				CurrentTextInKnownLanguageViewModel = dependencies.CreateOrPickTextInKnownLanguageViewModel,
				EditTextInStudiedLanguageIsEnabled = true,
				EditTextInKnownLanguageIsEnabled = false,
				TranslationFilter = "test text",
				FilteredTranslations = TestTranslationViewModels,
				SelectedTranslation = testTranslationViewModel,
			};

			target.Should().BeEquivalentTo(expectedViewModelData, EditDictionaryViewModelData.ExcludingCommands);

			dependencies.EditExistingTextInStudiedLanguageViewModelMock.Verify(x => x.Load(testTranslationViewModel.Translation.Text1, true, true, It.IsAny<CancellationToken>()), Times.Once);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.ClearFilledData(), Times.Once);
		}

		[TestMethod]
		public async Task EditLanguageTextInKnownLanguage_SwitchesViewModelsCorrectly()
		{
			// Arrange

			var testTranslationViewModel = TestTranslationViewModels[1];

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			dependencies.ClearInvocations();

			// Act

			// Emulating edit of ViewModels data that should not be cleared.
			target.TranslationFilter = "test text";

			await SwitchToEditTextInKnownLanguageMode(target, testTranslationViewModel);

			// Assert

			var expectedViewModelData = new EditDictionaryViewModelData
			{
				CurrentTextInStudiedLanguageViewModel = dependencies.CreateOrPickTextInStudiedLanguageViewModel,
				CurrentTextInKnownLanguageViewModel = dependencies.EditExistingTextInKnownLanguageViewModel,
				EditTextInStudiedLanguageIsEnabled = false,
				EditTextInKnownLanguageIsEnabled = true,
				TranslationFilter = "test text",
				FilteredTranslations = TestTranslationViewModels,
				SelectedTranslation = testTranslationViewModel,
			};

			target.Should().BeEquivalentTo(expectedViewModelData, EditDictionaryViewModelData.ExcludingCommands);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.ClearFilledData(), Times.Once);
			dependencies.EditExistingTextInKnownLanguageViewModelMock.Verify(x => x.Load(testTranslationViewModel.Translation.Text2, false, false, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteTranslation_DeletesTranslationCorrectly()
		{
			// Arrange

			var testTranslationViewModel = TestTranslationViewModels[1];

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Stubbing new translations that should be reloaded after deletion.
			dependencies.DictionaryServiceMock.Setup(x => x.GetTranslations(TestStudiedLanguage, TestKnownLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestNewTranslations);

			dependencies.ClearInvocations();

			// Act

			// Emulating edit of ViewModels data that should not be cleared.
			target.TranslationFilter = "test text";

			await ExecuteContextMenuCommand(target, testTranslationViewModel, "Delete Translation");

			// Assert

			dependencies.DictionaryServiceMock.Verify(x => x.DeleteTranslation(testTranslationViewModel.Translation, It.IsAny<CancellationToken>()), Times.Once);

			VerifyDataReloading(target, dependencies, TestNewTranslationViewModels);
		}

		[TestMethod]
		public async Task DeleteTextInStudiedLanguage_DeletesTextCorrectly()
		{
			// Arrange

			var testTranslationViewModel = TestTranslationViewModels[1];

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Stubbing new translations that should be reloaded after deletion.
			dependencies.DictionaryServiceMock.Setup(x => x.GetTranslations(TestStudiedLanguage, TestKnownLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestNewTranslations);

			dependencies.ClearInvocations();

			// Act

			// Emulating edit of ViewModels data that should not be cleared.
			target.TranslationFilter = "test text";

			await ExecuteContextMenuCommand(target, testTranslationViewModel, "Delete Text 'test text in studied language 2 (test note 2)' and 2 translation(s)");

			// Assert

			dependencies.DictionaryServiceMock.Verify(x => x.DeleteLanguageText(testTranslationViewModel.LanguageText1.LanguageText, It.IsAny<CancellationToken>()), Times.Once);
			dependencies.DictionaryServiceMock.Verify(x => x.DeleteLanguageText(testTranslationViewModel.LanguageText2.LanguageText, It.IsAny<CancellationToken>()), Times.Never);

			VerifyDataReloading(target, dependencies, TestNewTranslationViewModels);
		}

		[TestMethod]
		public async Task DeleteTextInKnownLanguage_DeletesTextCorrectly()
		{
			// Arrange

			var testTranslationViewModel = TestTranslationViewModels[1];

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Stubbing new translations that should be reloaded after deletion.
			dependencies.DictionaryServiceMock.Setup(x => x.GetTranslations(TestStudiedLanguage, TestKnownLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestNewTranslations);

			dependencies.ClearInvocations();

			// Act

			// Emulating edit of ViewModels data that should not be cleared.
			target.TranslationFilter = "test text";

			await ExecuteContextMenuCommand(target, testTranslationViewModel, "Delete Text 'test text in known language 1 (test note 1)' and 2 translation(s)");

			// Assert

			dependencies.DictionaryServiceMock.Verify(x => x.DeleteLanguageText(testTranslationViewModel.LanguageText1.LanguageText, It.IsAny<CancellationToken>()), Times.Never);
			dependencies.DictionaryServiceMock.Verify(x => x.DeleteLanguageText(testTranslationViewModel.LanguageText2.LanguageText, It.IsAny<CancellationToken>()), Times.Once);

			VerifyDataReloading(target, dependencies, TestNewTranslationViewModels);
		}

		[TestMethod]
		public async Task DeleteBothTexts_DeletesTextsCorrectly()
		{
			// Arrange

			var testTranslationViewModel = TestTranslationViewModels[1];

			var dependencies = new TestTargetDependencies();

			var target = CreateTestTarget(dependencies);

			await target.Load(TestStudiedLanguage, TestKnownLanguage, CancellationToken.None);

			// Stubbing new translations that should be reloaded after deletion.
			dependencies.DictionaryServiceMock.Setup(x => x.GetTranslations(TestStudiedLanguage, TestKnownLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestNewTranslations);

			dependencies.ClearInvocations();

			// Act

			// Emulating edit of ViewModels data that should not be cleared.
			target.TranslationFilter = "test text";

			await ExecuteContextMenuCommand(target, testTranslationViewModel, "Delete Both Texts and 3 translation(s)");

			// Assert

			dependencies.DictionaryServiceMock.Verify(x => x.DeleteLanguageText(testTranslationViewModel.LanguageText1.LanguageText, It.IsAny<CancellationToken>()), Times.Once);
			dependencies.DictionaryServiceMock.Verify(x => x.DeleteLanguageText(testTranslationViewModel.LanguageText2.LanguageText, It.IsAny<CancellationToken>()), Times.Once);

			VerifyDataReloading(target, dependencies, TestNewTranslationViewModels);
		}

		private EditDictionaryViewModel CreateTestTarget(TestTargetDependencies dependencies)
		{
			// We are not using AutoMocker because we don't see a way to inject different instances for dependencies of same type.

			dependencies.DictionaryServiceMock
				.Setup(x => x.GetTranslations(TestStudiedLanguage, TestKnownLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestTranslations);

			return new EditDictionaryViewModel(
				dependencies.DictionaryServiceMock.Object, Mock.Of<IMessenger>(),
				dependencies.CreateOrPickTextInStudiedLanguageViewModel, dependencies.CreateOrPickTextInKnownLanguageViewModel,
				dependencies.EditExistingTextInStudiedLanguageViewModel, dependencies.EditExistingTextInKnownLanguageViewModel);
		}

		private static Task SwitchToEditTextInStudiedLanguageMode(EditDictionaryViewModel target, TranslationViewModel translation)
		{
			return ExecuteContextMenuCommand(target, translation, $"Edit Text '{translation.LanguageText1.TextWithNote}'");
		}

		private static Task SwitchToEditTextInKnownLanguageMode(EditDictionaryViewModel target, TranslationViewModel translation)
		{
			return ExecuteContextMenuCommand(target, translation, $"Edit Text '{translation.LanguageText2.TextWithNote}'");
		}

		private static async Task ExecuteContextMenuCommand(EditDictionaryViewModel target, TranslationViewModel translation, string contextMenuItemHeader)
		{
			target.SelectedTranslation = translation;

			var contextMenuItems = target.GetContextMenuItemsForSelectedTranslation();
			var contextMenuItem = contextMenuItems
				.Single(x => x.Header == contextMenuItemHeader);

			await contextMenuItem.Command.ExecuteAsync(null);
		}

		private void VerifyDataReloading(EditDictionaryViewModel target, TestTargetDependencies dependencies, IReadOnlyList<TranslationViewModel> expectedTranslationViewModels)
		{
			var expectedViewModelData = new EditDictionaryViewModelData
			{
				CurrentTextInStudiedLanguageViewModel = dependencies.CreateOrPickTextInStudiedLanguageViewModel,
				CurrentTextInKnownLanguageViewModel = dependencies.CreateOrPickTextInKnownLanguageViewModel,
				EditTextInStudiedLanguageIsEnabled = true,
				EditTextInKnownLanguageIsEnabled = true,
				TranslationFilter = "test text",
				FilteredTranslations = expectedTranslationViewModels,
				SelectedTranslation = null,
			};

			target.Should().BeEquivalentTo(expectedViewModelData, EditDictionaryViewModelData.ExcludingCommands);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.Load(TestStudiedLanguage, true, true, It.IsAny<CancellationToken>()), Times.Once);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.Load(TestKnownLanguage, false, false, It.IsAny<CancellationToken>()), Times.Once);
			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.Verify(x => x.ClearFilledData(), Times.Never);
			dependencies.CreateOrPickTextInKnownLanguageViewModelMock.Verify(x => x.ClearFilledData(), Times.Never);

			dependencies.CreateOrPickTextInStudiedLanguageViewModelMock.VerifySet(x => x.TextIsFocused = true, Times.Once);
		}
	}
}
