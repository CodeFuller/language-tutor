using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.ViewModels;
using LanguageTutor.ViewModels.Exercises;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.Views.DesignInstances
{
	internal class EditExercisesDesignData : IEditExercisesViewModel
	{
		public ObservableCollection<InflectWordExerciseTypeViewModel> ExerciseTypes { get; }

		public InflectWordExerciseTypeViewModel SelectedExerciseType { get; set; }

		public string BaseForm { get; set; }

		public string Description { get; set; }

		public ObservableCollection<IEditInflectWordFormViewModel> WordFormViewModels { get; } = new();

		public IAsyncRelayCommand SaveChangesCommand => null;

		public ICommand ClearChangesCommand => null;

		public ICommand GoToStartPageCommand => null;

		public EditExercisesDesignData()
		{
			var exerciseTypes = new[]
			{
				new InflectWordExerciseTypeDescriptor
				{
					Title = "Odmiana czasownika w czasie teraźniejszym",
					DescriptionTemplate = new()
					{
						Id = new("1"),
						Template = "Proszę odmienić czasownik \"{BaseForm}\" w czasie teraźniejszym",
					},
					FormHints = [],
				},
			};

			ExerciseTypes = new ObservableCollection<InflectWordExerciseTypeViewModel>(exerciseTypes.Select(x => new InflectWordExerciseTypeViewModel(x)));
			SelectedExerciseType = ExerciseTypes.First();
			BaseForm = "być";
			Description = "Proszę odmienić czasownik \"być\" w czasie teraźniejszym";
		}

		public Task Load(Language studiedLanguage, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
