using System.Windows;
using VocabularyCoach.ViewModels;

namespace VocabularyCoach.Views
{
	public partial class ApplicationView : Window
	{
		public ApplicationView(ApplicationViewModel model)
		{
			InitializeComponent();
			DataContext = model;
		}
	}
}
