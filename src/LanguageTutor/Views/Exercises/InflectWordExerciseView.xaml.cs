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

		// We cannot access ViewModel from Unloaded event, because DataContext will be already unset.
		private IMessenger Messenger { get; set; }

		private bool HandlerIsRegistered => Messenger != null;

		public InflectWordExerciseView()
		{
			InitializeComponent();

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
