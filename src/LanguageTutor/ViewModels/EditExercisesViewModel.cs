using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises.Inflection;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Exercises;
using LanguageTutor.ViewModels.Extensions;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.ViewModels
{
	internal class EditExercisesViewModel : ObservableObject, IEditExercisesViewModel
	{
		private readonly IExerciseService exerciseService;

		public IMessenger Messenger { get; }

		private Language StudiedLanguage { get; set; }

		public ObservableCollection<InflectWordExerciseTypeViewModel> ExerciseTypes { get; } = new();

		private InflectWordExerciseTypeViewModel selectedExerciseType;

		public InflectWordExerciseTypeViewModel SelectedExerciseType
		{
			get => selectedExerciseType;
			set
			{
				SetProperty(ref selectedExerciseType, value);

				Description = GetDescriptionFromTemplate();

				WordFormViewModels.Clear();
				WordFormViewModels.AddRange(SelectedExerciseType.ExerciseTypeDescriptor.FormHints.Select(x => new EditInflectWordFormViewModel(x)));

				Messenger.Send(new InflectWordExerciseTypeSelectedEventArgs(SelectedExerciseType));
			}
		}

		private string baseForm;

		public string BaseForm
		{
			get => baseForm;
			set
			{
				SetProperty(ref baseForm, value);
				Description = GetDescriptionFromTemplate();
			}
		}

		private string description;

		public string Description
		{
			get => description;
			set => SetProperty(ref description, value);
		}

		public ObservableCollection<IEditInflectWordFormViewModel> WordFormViewModels { get; } = new();

		public IAsyncRelayCommand SaveChangesCommand { get; }

		public ICommand ClearChangesCommand { get; }

		public ICommand GoToStartPageCommand { get; }

		public EditExercisesViewModel(IExerciseService exerciseService, IMessenger messenger)
		{
			this.exerciseService = exerciseService ?? throw new ArgumentNullException(nameof(exerciseService));
			Messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			SaveChangesCommand = new AsyncRelayCommand(SaveChanges);
			ClearChangesCommand = new RelayCommand(ClearFilledData);
			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(Language studiedLanguage, CancellationToken cancellationToken)
		{
			var exerciseTypes = await exerciseService.GetInflectWordExerciseTypes(studiedLanguage, cancellationToken);

			StudiedLanguage = studiedLanguage;

			ExerciseTypes.Clear();
			ExerciseTypes.AddRange(exerciseTypes.Select(x => new InflectWordExerciseTypeViewModel(x)));

			SelectedExerciseType = ExerciseTypes.First();
			BaseForm = String.Empty;
		}

		private async Task SaveChanges(CancellationToken cancellationToken)
		{
			var exerciseData = new CreateInflectWordExerciseData
			{
				LanguageId = StudiedLanguage.Id,
				ExerciseTypeDescriptor = SelectedExerciseType.ExerciseTypeDescriptor,
				Description = Description,
				BaseForm = BaseForm,
				WordForms = WordFormViewModels.Select(x => new InflectWordForm
					{
						FormHint = x.FormHint,
						WordForm = x.WordForm,
					})
					.ToList(),
			};

			await exerciseService.AddInflectWordExercise(exerciseData, cancellationToken);

			ClearFilledData();
		}

		private void ClearFilledData()
		{
			BaseForm = String.Empty;

			foreach (var wordForm in WordFormViewModels)
			{
				wordForm.WordForm = String.Empty;
			}
		}

		private string GetDescriptionFromTemplate()
		{
			return SelectedExerciseType.ExerciseTypeDescriptor.GetDescription(BaseForm);
		}
	}
}
