using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FT_ProgramWPF.ViewModel
{
	public partial class ConnectionSetupViewModel : ObservableObject
	{

		public ConnectionSetupViewModel()
		{

		}

		public Action DisplayDownloadPage;


		[RelayCommand]
		async void PickFolder()
		{
			Debug.WriteLine("PickFolder!");
		}

		[RelayCommand]
		async void Host()
		{
			Debug.WriteLine("Host!");
		}

		[RelayCommand]
		async void Join()
		{

			await Application.Current.Dispatcher.InvokeAsync(DisplayDownloadPage);
			Debug.WriteLine("Join!");
			//DisplayDownloadPage.Invoke();
		}

	}
}
