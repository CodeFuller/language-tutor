using System.Windows.Controls;
using CodeFuller.Library.Wpf.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.ViewModels.Interfaces;
using LanguageTutor.Views.Exercises;

namespace LanguageTutor.Views
{
	public partial class EditExercisesView : UserControl
	{
		private IEditExercisesViewModel ViewModel => DataContext.GetViewModel<IEditExercisesViewModel>();

		// We save instance of IMessenger to property as we cannot access ViewModel.Messenger from Unloaded event, because DataContext will be already unset.
		private IMessenger Messenger { get; set; }

		private bool HandlerIsRegistered => Messenger != null;

		public EditExercisesView()
		{
			InitializeComponent();

			this.Loaded += (_, _) =>
			{
				if (HandlerIsRegistered)
				{
					return;
				}

				Messenger = ViewModel.Messenger;
				Messenger.Register<InflectWordExerciseTypeSelectedEventArgs>(this, (_, _) => UpdateWordForms());

				// InflectWordExerciseTypeSelectedEventArgs was already fired, so we have to call event handler manually first time.
				UpdateWordForms();
			};

			this.Unloaded += (_, _) =>
			{
				if (HandlerIsRegistered)
				{
					Messenger.Unregister<InflectWordExerciseTypeSelectedEventArgs>(this);
					Messenger = null;
				}
			};
		}

		private void UpdateWordForms()
		{
			WordForms.Children.Clear();

			foreach (var wordFormViewModel in ViewModel.WordFormViewModels)
			{
				var wordFormView = new EditInflectWordFormView
				{
					DataContext = wordFormViewModel,
				};

				WordForms.Children.Add(wordFormView);
			}
		}
	}
}
