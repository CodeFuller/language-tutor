using System;
using Microsoft.Extensions.DependencyInjection;
using VocabularyCoach.Services.GoogleTextToSpeech;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.Services.Internal;
using VocabularyCoach.Services.LanguageTraits;

namespace VocabularyCoach.Services
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddVocabularyCoachServices(this IServiceCollection services, Action<GoogleTextToSpeechApiSettings> setupSettings)
		{
			services.Configure(setupSettings);

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
	}
}
