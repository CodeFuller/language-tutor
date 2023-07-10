using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.Interfaces
{
	public interface IPronunciationRecordPlayer
	{
		bool PronunciationRecordDataIsCorrect(PronunciationRecord pronunciationRecord, out string errorMessage);

		Task PlayPronunciationRecord(PronunciationRecord pronunciationRecord, CancellationToken cancellationToken);
	}
}
