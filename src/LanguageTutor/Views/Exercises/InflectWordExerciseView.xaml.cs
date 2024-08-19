using System.Windows.Controls;
using CodeFuller.Library.Wpf.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.ViewModels.Exercises;

namespace LanguageTutor.Views.Exercises
{
	public partial class InflectWordExerciseView : UserControl
	{
		private IInflectWordExerciseViewModel ViewModel => DataContext.GetViewModel<IInflectWordExerciseViewModel>();

		// We save instance of IMessenger to property as we cannot access ViewModel.Messenger from Unloaded event, because DataContext will be already unset.
		private IMessenger Messenger { get; set; }

		private bool HandlerIsRegistered => Messenger != null;

		public InflectWordExerciseView()
		{
			InitializeComponent();

			// It is non-trivial to update WordForms when ViewModel collection changes.
			// The challenges are:
			//
			//  1. We cannot update view just once in the handler of Loaded event, because Loaded event will not be fired again if new exercise is loaded.
			//     So we must somehow subscribe to update event from View Model.
			//
			//  2. First time when view is loaded, the View constructor is called after ViewModel.Load() method.
			//     Most likely, this happens because ApplicationView uses ContentControl with template based on ViewModel data type.
			//
			//  3. When Unloaded event is fired, ViewModel is already unset.
			//
			//  Below code handles all described challenges.
			this.Loaded += (_, _) =>
			{
				if (HandlerIsRegistered)
				{
					return;
				}

				Messenger = ViewModel.Messenger;
				Messenger.Register<InflectWordExerciseLoadedEventArgs>(this, (_, _) => UpdateWordForms());

				// InflectWordExerciseLoadedEventArgs was already fired, so we have to call event handler manually first time.
				UpdateWordForms();
			};

			this.Unloaded += (_, _) =>
			{
				if (HandlerIsRegistered)
				{
					Messenger.Unregister<InflectWordExerciseLoadedEventArgs>(this);
					Messenger = null;
				}
			};
		}

		private void UpdateWordForms()
		{
			WordForms.Children.Clear();

			foreach (var wordFormViewModel in ViewModel.WordFormViewModels)
			{
				var wordFormView = new InflectWordFormView
				{
					DataContext = wordFormViewModel,
				};

				WordForms.Children.Add(wordFormView);
			}
		}
	}
}
