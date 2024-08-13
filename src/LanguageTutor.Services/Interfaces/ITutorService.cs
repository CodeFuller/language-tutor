using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Data;

namespace LanguageTutor.Services.Interfaces
{
	public interface ITutorService
	{
		Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken);

		Task<IReadOnlyCollection<BasicExercise>> GetExercisesToPerform(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<BasicExercise>> GetProblematicExercises(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);

		Task<PronunciationRecord> GetPronunciationRecord(ItemId textId, CancellationToken cancellationToken);

		Task<UserStatisticsData> GetTodayUserStatistics(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);

		Task UpdateTodayUserStatistics(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<UserStatisticsData>> GetUserStatisticsHistory(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
