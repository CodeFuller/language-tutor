using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Models.Exercises;
using LanguageTutor.ViewModels.Exercises;

namespace LanguageTutor.Views.DesignInstances.Exercises
{
	internal class InflectWordExerciseDesignData : IInflectWordExerciseViewModel
	{
		public IMessenger Messenger => null;

		public InflectWordExercise Exercise { get; } = new(new("1"), new DateTimeOffset(2024, 08, 14, 18, 07, 21, TimeSpan.Zero), "Proszę odmienić czasownik \"być\" w czasie teraźniejszym", "być", [], []);

		public string Description => Exercise.Description;

		public ObservableCollection<IInflectWordFormViewModel> WordFormViewModels { get; } = new();

		public bool ExerciseWasChecked => false;

		public Task<BasicExerciseResult> CheckExercise(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
