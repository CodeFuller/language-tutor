using Microsoft.Extensions.DependencyInjection;
using VocabularyCoach.Abstractions.Interfaces;
using VocabularyCoach.Abstractions.LanguageTraits;

namespace VocabularyCoach.Abstractions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddVocabularyCoachServices(this IServiceCollection services)
		{
			services.AddSingleton<InMemoryVocabularyService>();
			services.AddSingleton<IVocabularyService>(sp => sp.GetRequiredService<InMemoryVocabularyService>());
			services.AddSingleton<IEditVocabularyService>(sp => sp.GetRequiredService<InMemoryVocabularyService>());

			services.AddSingleton<ILanguageTraits, PolishLanguageTraits>();

			return services;
		}
	}
}
