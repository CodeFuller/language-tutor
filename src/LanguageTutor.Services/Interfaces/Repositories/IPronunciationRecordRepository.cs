using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;

namespace LanguageTutor.Services.Interfaces.Repositories
{
	public interface IPronunciationRecordRepository
	{
		Task<PronunciationRecord> GetPronunciationRecord(ItemId languageTextId, CancellationToken cancellationToken);

		Task AddPronunciationRecord(ItemId languageTextId, PronunciationRecord pronunciationRecord, CancellationToken cancellationToken);

		Task UpdatePronunciationRecord(ItemId languageTextId, PronunciationRecord pronunciationRecord, CancellationToken cancellationToken);
	}
}
