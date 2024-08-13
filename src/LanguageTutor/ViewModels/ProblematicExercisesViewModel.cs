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
	public class ProblematicExercisesViewModel : IProblematicExercisesViewModel
	{
		private readonly ITutorService tutorService;

		public ObservableCollection<ProblematicExerciseViewModel> ProblematicExercises { get; } = new();

		public ProblematicExerciseViewModel SelectedExercise { get; set; }

		public ICommand GoToStartPageCommand { get; }

		public ProblematicExercisesViewModel(ITutorService tutorService, IMessenger messenger)
		{
			this.tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));

			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var problematicExercises = await tutorService.GetProblematicExercises(user, studiedLanguage, knownLanguage, cancellationToken);

			ProblematicExercises.Clear();
			ProblematicExercises.AddRange(problematicExercises.Select(x => new ProblematicExerciseViewModel(x)));

			SelectedExercise = ProblematicExercises.FirstOrDefault();
		}
	}
}
