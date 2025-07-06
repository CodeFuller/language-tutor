using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LanguageTutor.ViewModels.Collections
{
	// https://stackoverflow.com/a/13303245/5740031
	internal class SmartObservableCollection<T> : ObservableCollection<T>
	{
		public void AddRange(IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				Items.Add(item);
			}

			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}
}
