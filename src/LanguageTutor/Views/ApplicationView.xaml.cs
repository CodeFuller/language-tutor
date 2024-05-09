using System.Windows;
using LanguageTutor.ViewModels;

namespace LanguageTutor.Views
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
