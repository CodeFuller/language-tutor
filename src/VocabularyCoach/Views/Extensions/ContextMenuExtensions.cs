using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using VocabularyCoach.ViewModels.ContextMenu;

namespace VocabularyCoach.Views.Extensions
{
	internal static class ContextMenuExtensions
	{
		public static ContextMenu ToContextMenu(this IEnumerable<ContextMenuItem> menuItems)
		{
			var contextMenu = new ContextMenu();
			foreach (var menuItem in menuItems.Select(x => x.GetMenuItem()))
			{
				contextMenu.Items.Add(menuItem);
			}

			return contextMenu;
		}
	}
}
