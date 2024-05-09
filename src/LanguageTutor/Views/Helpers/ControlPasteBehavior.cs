using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LanguageTutor.Views.Helpers
{
	// https://stackoverflow.com/a/28365540/5740031
	public static class ControlPasteBehavior
	{
		public static readonly DependencyProperty PasteCommandProperty =
			DependencyProperty.RegisterAttached("PasteCommand", typeof(ICommand), typeof(ControlPasteBehavior), new FrameworkPropertyMetadata(PasteCommandChanged));

		public static ICommand GetPasteCommand(DependencyObject target)
		{
			return (ICommand)target.GetValue(PasteCommandProperty);
		}

		public static void SetPasteCommand(DependencyObject target, ICommand value)
		{
			target.SetValue(PasteCommandProperty, value);
		}

		private static void PasteCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var control = (Control)sender;
			var newValue = (ICommand)e.NewValue;

			if (newValue != null)
			{
				control.AddHandler(CommandManager.ExecutedEvent, new RoutedEventHandler(CommandExecuted), true);
			}
			else
			{
				control.RemoveHandler(CommandManager.ExecutedEvent, new RoutedEventHandler(CommandExecuted));
			}
		}

		private static void CommandExecuted(object sender, RoutedEventArgs e)
		{
			if (((ExecutedRoutedEventArgs)e).Command != ApplicationCommands.Paste)
			{
				return;
			}

			var control = (Control)sender;
			var command = GetPasteCommand(control);

			if (command.CanExecute(null))
			{
				command.Execute(control);
			}
		}
	}
}
