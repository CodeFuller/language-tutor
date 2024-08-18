using System.Windows.Controls;
using System.Windows.Input;
using CodeFuller.Library.Wpf.Extensions;
using LanguageTutor.ViewModels.Exercises;

namespace LanguageTutor.Views.Exercises
{
	public partial class InflectWordFormView : UserControl
	{
		private IInflectWordFormViewModel ViewModel => DataContext.GetViewModel<IInflectWordFormViewModel>();

		public InflectWordFormView()
		{
			InitializeComponent();
		}

		private void TypedWordGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			// We cannot rely solely on setting focus from View Model via SetFocus helper,
			// because user can manually change focus to another word form.
			ViewModel.TypedWordIsFocused = true;
		}

		private void TypedWordLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			ViewModel.TypedWordIsFocused = false;
		}
	}
}
