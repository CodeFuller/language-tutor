using System.Windows.Controls;
using CodeFuller.Library.Wpf.Extensions;
using LanguageTutor.ViewModels.Interfaces;
using LanguageTutor.Views.Extensions;

namespace LanguageTutor.Views
{
	public partial class EditDictionaryView : UserControl
	{
		private IEditDictionaryViewModel ViewModel => DataContext.GetViewModel<IEditDictionaryViewModel>();

		public EditDictionaryView()
		{
			InitializeComponent();

			// Without this, context menu will not open first time.
			TranslationsListBox.ContextMenu = new ContextMenu();
			TranslationsListBox.ContextMenuOpening += TranslationsListBox_ContextMenuOpening;
		}

		private void TranslationsListBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			TranslationsListBox.ContextMenu = ViewModel.GetContextMenuItemsForSelectedTranslation().ToContextMenu();
		}
	}
}
