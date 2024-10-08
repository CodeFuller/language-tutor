using CodeFuller.Library.Bootstrap;
using CodeFuller.Library.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LanguageTutor.MergeDatabases
{
	internal sealed class ApplicationBootstrapper : BasicApplicationBootstrapper<IApplicationLogic>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddSingleton<IApplicationLogic, ApplicationLogic>();
		}

		protected override void BootstrapLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			loggerFactory.AddLogging(settings => configuration.Bind("logging", settings));
		}
	}
}
