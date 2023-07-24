using System.Windows.Controls;
using System.Windows.Input;

namespace VocabularyCoach.ViewModels.ContextMenu
{
	public class ContextMenuItem
	{
		public string Header { get; init; }

		public ICommand Command { get; init; }

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
