using Microsoft.Extensions.DependencyInjection;
using VocabularyCoach.Abstractions.Interfaces;

namespace VocabularyCoach.Abstractions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddVocabularyCoachServices(this IServiceCollection services)
		{
			services.AddSingleton<IVocabularyService, InMemoryVocabularyService>();

			return services;
		}
	}
}
