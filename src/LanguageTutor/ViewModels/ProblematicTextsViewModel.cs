using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.Models;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Extensions;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.ViewModels
{
	public class ProblematicTextsViewModel : IProblematicTextsViewModel
	{
		private readonly ITutorService tutorService;

		public ObservableCollection<ProblematicTextViewModel> ProblematicTexts { get; } = new();

		public ProblematicTextViewModel SelectedText { get; set; }

		public ICommand GoToStartPageCommand { get; }

		public ProblematicTextsViewModel(ITutorService tutorService, IMessenger messenger)
		{
			this.tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));

			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var problematicTexts = await tutorService.GetProblematicTexts(user, studiedLanguage, knownLanguage, cancellationToken);

			ProblematicTexts.Clear();
			ProblematicTexts.AddRange(problematicTexts.Select(x => new ProblematicTextViewModel(x)));

			SelectedText = ProblematicTexts.FirstOrDefault();
		}
	}
}
