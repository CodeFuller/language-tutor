using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Interfaces.Repositories
{
	public interface IPronunciationRecordRepository
	{
		Task<PronunciationRecord> GetPronunciationRecord(ItemId languageTextId, CancellationToken cancellationToken);

		Task AddPronunciationRecord(ItemId languageTextId, PronunciationRecord pronunciationRecord, CancellationToken cancellationToken);
	}
}
