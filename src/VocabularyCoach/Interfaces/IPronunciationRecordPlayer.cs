using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.Interfaces
{
	public interface IPronunciationRecordPlayer
	{
		Task PlayPronunciationRecord(PronunciationRecord pronunciationRecord, CancellationToken cancellationToken);
	}
}
