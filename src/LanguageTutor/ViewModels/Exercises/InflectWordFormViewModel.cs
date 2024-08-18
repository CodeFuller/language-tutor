using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using static LanguageTutor.ViewModels.Extensions.FocusHelpers;

namespace LanguageTutor.ViewModels.Exercises
{
	public class InflectWordFormViewModel : ObservableObject, IInflectWordFormViewModel
	{
		public string FormHint { get; }

		public string CorrectWordForm { get; }

		private string typedWordForm;

		public string TypedWordForm
		{
			get => typedWordForm;
			set => SetProperty(ref typedWordForm, value);
		}

		private bool typedWordIsFocused;

		public bool TypedWordIsFocused
		{
			get => typedWordIsFocused;
			set => SetProperty(ref typedWordIsFocused, value);
		}

		private bool wordFormWasChecked;

		public bool WordFormWasChecked
		{
			get => wordFormWasChecked;
			set => SetProperty(ref wordFormWasChecked, value);
		}

		private bool wordFormWasTypedCorrectly;

		public bool WordFormIsTypedCorrectly
		{
			get => wordFormWasTypedCorrectly;
			set => SetProperty(ref wordFormWasTypedCorrectly, value);
		}

		private bool wordFormWasTypedIncorrectly;

		public bool WordFormIsTypedIncorrectly
		{
			get => wordFormWasTypedIncorrectly;
			set => SetProperty(ref wordFormWasTypedIncorrectly, value);
		}

		public ICommand NextStepCommand { get; }

		public InflectWordFormViewModel(IMessenger messenger, string formHint, string correctWordForm)
		{
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			FormHint = formHint;
			CorrectWordForm = correctWordForm;

			NextStepCommand = new RelayCommand(() => messenger.Send(new NextStepWithinExerciseEventArgs()));
		}

		public void Load()
		{
			SetFocus(() => TypedWordIsFocused);
		}
	}
}
