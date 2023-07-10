using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.Interfaces
{
	public interface IPronunciationRecordLoader
	{
		Task<PronunciationRecord> LoadPronunciationRecord(string dataSource, CancellationToken cancellationToken);
	}
}
