using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;

namespace LanguageTutor.Interfaces
{
	public interface IPronunciationRecordPlayer
	{
		Task PlayPronunciationRecord(PronunciationRecord pronunciationRecord, CancellationToken cancellationToken);
	}
}
