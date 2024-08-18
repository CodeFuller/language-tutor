using CodeFuller.Library.Bootstrap;
using CodeFuller.Library.Logging;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Infrastructure.Sqlite;
using LanguageTutor.Interfaces;
using LanguageTutor.Internal;
using LanguageTutor.Services;
using LanguageTutor.Settings;
using LanguageTutor.ViewModels;
using LanguageTutor.ViewModels.Exercises;
using LanguageTutor.ViewModels.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LanguageTutor
{
	internal class ApplicationBootstrapper : BasicApplicationBootstrapper<ApplicationViewModel>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			RegisterViewModels(services, configuration);

			services.AddLanguageTutorServices(configuration.Bind);
			services.AddLanguageTutorSqliteDal(settings => configuration.Bind("database", settings));

			services.AddSingleton<IPronunciationRecordPlayer, PronunciationRecordPlayer>();
			services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
		}

		private static void RegisterViewModels(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<ApplicationSettings>(configuration.Bind);

			services.AddSingleton<ApplicationViewModel>();

			services.AddSingleton<IStartPageViewModel, StartPageViewModel>();
			services.AddSingleton<IPerformExercisesViewModel, PerformExercisesViewModel>();
			services.AddSingleton<IExerciseResultsViewModel, ExerciseResultsViewModel>();
			services.AddSingleton<IEditDictionaryViewModel, EditDictionaryViewModel>();
			services.AddSingleton<IProblematicExercisesViewModel, ProblematicExercisesViewModel>();
			services.AddSingleton<IStatisticsChartViewModel, StatisticsChartViewModel>();

			services.AddSingleton<IExerciseViewModel, TranslateTextExerciseViewModel>();
			services.AddSingleton<IExerciseViewModel, InflectWordExerciseViewModel>();

			// We register ICreateOrPickTextViewModel as transient dependency, because two different instances must be injected into EditDictionaryViewModel.
			services.AddTransient<ICreateOrPickTextViewModel, CreateOrPickTextViewModel>();

			// We register IEditExistingTextViewModel as transient dependency, because two different instances must be injected into EditDictionaryViewModel.
			services.AddTransient<IEditExistingTextViewModel, EditExistingTextViewModel>();
		}

		protected override void BootstrapLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			loggerFactory.AddLogging(settings => configuration.Bind("logging", settings));
		}
	}
}
