using CommunityToolkit.Mvvm.ComponentModel;

namespace LanguageTutor.ViewModels.Exercises
{
	public class EditInflectWordFormViewModel : ObservableObject, IEditInflectWordFormViewModel
	{
		public string FormHint { get; }

		private string wordForm;

		public string WordForm
		{
			get => wordForm;
			set => SetProperty(ref wordForm, value);
		}

		public EditInflectWordFormViewModel(string formHint)
		{
			FormHint = formHint;
		}
	}
}
