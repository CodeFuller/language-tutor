using System.Windows.Controls;
using CodeFuller.Library.Wpf.Extensions;
using VocabularyCoach.ViewModels.Interfaces;
using VocabularyCoach.Views.Extensions;

namespace VocabularyCoach.Views
{
	public partial class EditVocabularyView : UserControl
	{
		private IEditVocabularyViewModel ViewModel => DataContext.GetViewModel<IEditVocabularyViewModel>();

		public EditVocabularyView()
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
