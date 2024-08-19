using LanguageTutor.ViewModels.Exercises;

namespace LanguageTutor.Views.DesignInstances.Exercises
{
	internal class EditInflectWordFormDesignData : IEditInflectWordFormViewModel
	{
		public string FormHint => "ja";

		public string WordForm { get; set; } = "jestem";
	}
}
