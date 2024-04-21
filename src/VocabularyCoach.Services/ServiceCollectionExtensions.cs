using System;
using Microsoft.Extensions.DependencyInjection;
using VocabularyCoach.Services.GoogleTextToSpeech;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.Services.Internal;
using VocabularyCoach.Services.LanguageTraits;
using VocabularyCoach.Services.Settings;

namespace VocabularyCoach.Services
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddVocabularyCoachServices(this IServiceCollection services, Action<VocabularyCoachSettings> setupSettings)
		{
			services.Configure(AdaptSettings<VocabularyCoachSettings, PracticeSettings>(setupSettings, (vocabularyCoachSettings, practiceSettings) => vocabularyCoachSettings.Practice = practiceSettings));
			services.Configure(AdaptSettings<VocabularyCoachSettings, GoogleTextToSpeechApiSettings>(setupSettings, (vocabularyCoachSettings, googleTextToSpeechApiSettings) => vocabularyCoachSettings.GoogleTextToSpeechApi = googleTextToSpeechApiSettings));

			services.AddSingleton<IUserService, UserService>();
			services.AddSingleton<IVocabularyService, VocabularyService>();
			services.AddSingleton<IEditVocabularyService, EditVocabularyService>();

			services.AddSingleton<ILanguageTraits, PolishLanguageTraits>();
			services.AddSingleton<ISupportedLanguageTraits, SupportedLanguageTraits>();

			services.AddHttpClient<IPronunciationRecordSynthesizer, GoogleTextToSpeechSynthesizer>();

			services.AddSingleton<ITextsForPracticeSelector, TextsForPracticeSelector>();
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
