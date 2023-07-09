using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.Interfaces
{
	public interface IPronunciationRecordPlayer
	{
		Task PlayPronunciationRecord(PronunciationRecord pronunciationRecord, CancellationToken cancellationToken);
	}
}
