using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace LanguageTutor.UnitTests.ViewModels
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

		[DataRow(0, 0, "0")]
		[DataRow(123, 123, "123")]
		[DataRow(123, 456, "123 (456)")]
		[DataRow(0, 123, "0 (123)")]
		[TestMethod]
		public async Task RestNumberOfExercisesToPerformTodayGetter_ReturnsCorrectValue(int restNumberOfExercisesToPerformToday, int restNumberOfExercisesToPerformTodayIfNoLimit, string expectedValue)
		{
			// Arrange

			var user = new User();

			var userSettings = new UserSettingsData
			{
				LastStudiedLanguage = TestStudiedLanguage,
				LastKnownLanguage = TestKnownLanguage,
			};

			var userStatistics = new UserStatisticsData
			{
				RestNumberOfExercisesToPerformToday = restNumberOfExercisesToPerformToday,
				RestNumberOfExercisesToPerformTodayIfNoLimit = restNumberOfExercisesToPerformTodayIfNoLimit,
			};

			var mocker = new AutoMocker();

			var tutorServiceMock = mocker.GetMock<ITutorService>();
			tutorServiceMock.Setup(x => x.GetLanguages(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { TestStudiedLanguage, TestKnownLanguage });

			mocker.GetMock<IUserService>().Setup(x => x.GetUserSettings(user, It.IsAny<CancellationToken>())).ReturnsAsync(userSettings);

			tutorServiceMock.Setup(x => x.GetTodayUserStatistics(It.IsAny<User>(), It.IsAny<Language>(), It.IsAny<Language>(), It.IsAny<CancellationToken>())).ReturnsAsync(userStatistics);

			var target = mocker.CreateInstance<StartPageViewModel>();

			await target.Load(user, CancellationToken.None);

			// Act

			var result = target.RestNumberOfExercisesToPerformToday;

			// Assert

			result.Should().Be(expectedValue);
		}

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

			var tutorServiceMock = mocker.GetMock<ITutorService>();
			tutorServiceMock.Setup(x => x.GetLanguages(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { TestStudiedLanguage, TestKnownLanguage });

			mocker.GetMock<IUserService>().Setup(x => x.GetUserSettings(user, It.IsAny<CancellationToken>())).ReturnsAsync(userSettings);

			var target = mocker.CreateInstance<StartPageViewModel>();

			await target.Load(user, CancellationToken.None);
			tutorServiceMock.Invocations.Clear();

			// Act

			await target.Load(user, CancellationToken.None);

			// Assert

			tutorServiceMock.Verify(x => x.GetTodayUserStatistics(It.IsAny<User>(), It.IsAny<Language>(), It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
