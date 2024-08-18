using System;
using System.Windows.Input;
using LanguageTutor.ViewModels.Exercises;

namespace LanguageTutor.Views.DesignInstances.Exercises
{
	internal class InflectWordFormDesignData : IInflectWordFormViewModel
	{
		public string FormHint => "ja";

		public string CorrectWordForm => "jestem";

		public string TypedWordForm { get; set; } = "jesten";

		public bool TypedWordIsFocused { get; set; } = true;

		public bool WordFormWasChecked => true;

		public bool WordFormIsTypedCorrectly { get; }

		public bool WordFormIsTypedIncorrectly { get; } = true;

		public ICommand NextStepCommand => null;

		public void Load()
		{
			throw new NotImplementedException();
		}
	}
}
