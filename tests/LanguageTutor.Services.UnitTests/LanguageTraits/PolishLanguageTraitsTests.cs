using System.Linq;
using FluentAssertions;
using LanguageTutor.Services.LanguageTraits;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace LanguageTutor.Services.UnitTests.LanguageTraits
{
	[TestClass]
	public class PolishLanguageTraitsTests
	{
		[TestMethod]
		public void GetInflectWordExerciseTypes_ForDescriptionTemplates_ReturnsUniqueIds()
		{
			// Arrange

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<PolishLanguageTraits>();

			// Act

			var descriptionTemplateIds = target.GetInflectWordExerciseTypes()
				.Select(x => x.DescriptionTemplate)
				.Where(x => x != null)
				.Select(x => x.Id);

			// Assert

			descriptionTemplateIds.Should().OnlyHaveUniqueItems();
		}
	}
}
