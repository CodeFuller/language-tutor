using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Interfaces
{
	public interface IPronunciationRecordSynthesizer
	{
		Task<PronunciationRecord> SynthesizePronunciationRecord(Language language, string text, CancellationToken cancellationToken);
	}
}
