using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.UnitTests
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
		public void RegisterServices_RegistersDependenciesForEditVocabularyViewModelCorrectly()
		{
			// Arrange

			using var target = new TestApplicationBootstrapper();

			// Act

			target.Bootstrap(Array.Empty<string>());

			// Assert

			// Constructor of EditVocabularyViewModel will throw if same instances of inner view models are injected.
			target.InvokeResolve<IEditVocabularyViewModel>();
		}
	}
}
