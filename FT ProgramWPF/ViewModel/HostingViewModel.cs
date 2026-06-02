using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FT_ProgramWPF.ViewModel
{
	public partial class HostingViewModel : ObservableObject
	{
		public ObservableCollection<string> LogEntries { get; } = new ObservableCollection<string>();

		public ScrollViewer LogScrollViewer { get; set; }

		public HostingViewModel()
		{

			//LogEntries.CollectionChanged += LogEntries_CollectionChanged;
			Application.Current.Dispatcher.Invoke(async () =>
			{

				for (int i = 0; i < 100; i++)
				{
					AddLogEntry($"{i}: Hello ffkmr rfkrkmfkmr krkf kr kf kr kf kr " +
						$"rkflmrmfmmeomofm4ompofmmfomopm emfopmewpofmopmw fowmpofm wmfwmpo" +
						$"wofmwkrefm fowmpfmwpf wmfpwm fjwmporm wmfpwmmrw" +
						$"wormpwmef wekrpkmrfmwplefmwepokrr. END!");

					await Task.Delay(300);
				}
			});
		}

		public void AddLogEntry(string text)
		{
			LogEntries.Add(text);
		}

		//private void LogEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		//{
		//	if (e.Action == NotifyCollectionChangedAction.Add)
		//	{
		//		// Use Dispatcher to ensure UI is updated before scrolling
		//		Application.Current.Dispatcher.InvokeAsync(() =>
		//		{
		//			LogScrollViewer.ScrollToEnd();
		//		}, System.Windows.Threading.DispatcherPriority.Loaded);
		//	}
		//}
	}
}
