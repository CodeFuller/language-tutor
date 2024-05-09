using LanguageTutor.ViewModels.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanguageTutor.UnitTests
{
	[TestClass]
	public class ApplicationBootstrapperTests
	{
		private sealed class TestApplicationBootstrapper : ApplicationBootstrapper
		{
			public T InvokeResolve<T>()
			{
				return Resolve<T>();
			}
		}

		[TestMethod]
		public void RegisterServices_RegistersDependenciesForEditDictionaryViewModelCorrectly()
		{
			// Arrange

			using var target = new TestApplicationBootstrapper();

			// Act

			target.Bootstrap(new[] { "userId=test" });

			// Assert

			// Constructor of EditDictionaryViewModel will throw if same instances of inner view models are injected.
			target.InvokeResolve<IEditDictionaryViewModel>();
		}
	}
}
