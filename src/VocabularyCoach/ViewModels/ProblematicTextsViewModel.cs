using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Events;
using VocabularyCoach.Extensions;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class ProblematicTextsViewModel : IProblematicTextsViewModel
	{
		private readonly IVocabularyService vocabularyService;

		public ObservableCollection<ProblematicTextViewModel> ProblematicTexts { get; } = new();

		public ProblematicTextViewModel SelectedText { get; set; }

		public ICommand GoToStartPageCommand { get; }

		public ProblematicTextsViewModel(IVocabularyService vocabularyService, IMessenger messenger)
		{
			this.vocabularyService = vocabularyService ?? throw new ArgumentNullException(nameof(vocabularyService));

			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var problematicTexts = await vocabularyService.GetProblematicTexts(user, studiedLanguage, knownLanguage, cancellationToken);

			ProblematicTexts.Clear();
			ProblematicTexts.AddRange(problematicTexts.Select(x => new ProblematicTextViewModel(x)));

			SelectedText = ProblematicTexts.FirstOrDefault();
		}
	}
}
