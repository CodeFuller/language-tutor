using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;

namespace LanguageTutor.Services.Interfaces
{
	public interface IPronunciationRecordSynthesizer
	{
		Task<PronunciationRecord> SynthesizePronunciationRecord(Language language, string text, CancellationToken cancellationToken);
	}
}
