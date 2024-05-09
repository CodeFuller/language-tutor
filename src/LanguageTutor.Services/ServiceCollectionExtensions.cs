using System;
using LanguageTutor.Services.GoogleTextToSpeech;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.Services.Internal;
using LanguageTutor.Services.LanguageTraits;
using LanguageTutor.Services.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageTutor.Services
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddLanguageTutorServices(this IServiceCollection services, Action<LanguageTutorSettings> setupSettings)
		{
			services.Configure(AdaptSettings<LanguageTutorSettings, PracticeSettings>(setupSettings, (languageTutorSettings, practiceSettings) => languageTutorSettings.Practice = practiceSettings));
			services.Configure(AdaptSettings<LanguageTutorSettings, GoogleTextToSpeechApiSettings>(setupSettings, (languageTutorSettings, googleTextToSpeechApiSettings) => languageTutorSettings.GoogleTextToSpeechApi = googleTextToSpeechApiSettings));

			services.AddSingleton<IUserService, UserService>();
			services.AddSingleton<ITutorService, TutorService>();
			services.AddSingleton<IDictionaryService, DictionaryService>();

			services.AddSingleton<ILanguageTraits, PolishLanguageTraits>();
			services.AddSingleton<ISupportedLanguageTraits, SupportedLanguageTraits>();

			services.AddHttpClient<IPronunciationRecordSynthesizer, GoogleTextToSpeechSynthesizer>();

			services.AddSingleton<ITextsForPracticeSelector, TextsForPracticeSelector>();
			services.AddSingleton<INextCheckDateProvider, NextCheckDateProvider>();

			services.AddSingleton<IProblematicTextsSelector, ProblematicTextsSelector>();

			services.AddSingleton<ISynonymGrouper, SynonymGrouper>();
			services.AddSingleton<ISystemClock, SystemClock>();

			services.AddSingleton<ISpellCheckService, SpellCheckService>();
			services.AddSingleton<IWebBrowser, DefaultSystemWebBrowser>();

			return services;
		}

		private static Action<TChildSettings> AdaptSettings<TParentSettings, TChildSettings>(Action<TParentSettings> setupParentSettings, Action<TParentSettings, TChildSettings> injectChildSettings)
			where TParentSettings : new()
		{
			return childSettings =>
			{
				var parentSettings = new TParentSettings();

				injectChildSettings(parentSettings, childSettings);

				setupParentSettings(parentSettings);
			};
		}
	}
}
