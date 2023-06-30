using CodeFuller.Library.Bootstrap;
using CodeFuller.Library.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VocabularyCoach.ViewModels;

namespace VocabularyCoach
{
	internal sealed class ApplicationBootstrapper : BasicApplicationBootstrapper<ApplicationViewModel>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddSingleton<ApplicationViewModel>();
		}

		protected override void BootstrapLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			loggerFactory.AddLogging(settings => configuration.Bind("logging", settings));
		}
	}
}
