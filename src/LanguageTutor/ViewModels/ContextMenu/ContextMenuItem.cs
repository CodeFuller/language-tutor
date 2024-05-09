using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;

namespace LanguageTutor.ViewModels.ContextMenu
{
	public class ContextMenuItem
	{
		public string Header { get; init; }

		public IAsyncRelayCommand Command { get; init; }

#pragma warning disable CA1024 // Use properties where appropriate
		public MenuItem GetMenuItem()
#pragma warning restore CA1024 // Use properties where appropriate
		{
			return new()
			{
				Header = Header,
				Command = Command,
			};
		}
	}
}
