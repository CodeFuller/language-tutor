using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;

namespace LanguageTutor.ViewModels.Interfaces
{
	public interface IEditExistingTextViewModel : IBasicEditTextViewModel
	{
		Task Load(LanguageText editedLanguageText, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken);
	}
}
