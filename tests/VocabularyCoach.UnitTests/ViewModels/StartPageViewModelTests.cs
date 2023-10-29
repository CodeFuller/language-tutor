using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.ViewModels;

namespace VocabularyCoach.UnitTests.ViewModels
{
	[TestClass]
	public class StartPageViewModelTests
	{
		private Language TestStudiedLanguage { get; } = new()
		{
			Id = new ItemId("Test Studied Language"),
			Name = "Test Studied Language",
		};

		private Language TestKnownLanguage { get; } = new()
		{
			Id = new ItemId("Test Known Language"),
			Name = "Test Known Language",
		};

		[TestMethod]
		public async Task Load_CalledRepeatedly_LoadsUserStatisticsOnce()
		{
			// Arrange

			var user = new User();

			var userSettings = new UserSettingsData
			{
				LastStudiedLanguage = TestStudiedLanguage,
				LastKnownLanguage = TestKnownLanguage,
			};

			var mocker = new AutoMocker();

			var vocabularyServiceMock = mocker.GetMock<IVocabularyService>();
			vocabularyServiceMock.Setup(x => x.GetLanguages(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { TestStudiedLanguage, TestKnownLanguage });

			mocker.GetMock<IUserService>().Setup(x => x.GetUserSettings(user, It.IsAny<CancellationToken>())).ReturnsAsync(userSettings);

			var target = mocker.CreateInstance<StartPageViewModel>();

			await target.Load(user, CancellationToken.None);
			vocabularyServiceMock.Invocations.Clear();

			// Act

			await target.Load(user, CancellationToken.None);

			// Assert

			vocabularyServiceMock.Verify(x => x.GetTodayUserStatistics(It.IsAny<User>(), It.IsAny<Language>(), It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
