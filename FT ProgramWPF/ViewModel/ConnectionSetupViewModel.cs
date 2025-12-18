using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
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

		private string _outputFolderPath;
		public Action DisplayDownloadPage;

		public string OutputPath {  get; set; }

		[RelayCommand]
		async void PickFolder()
		{
			Debug.WriteLine("PickFolder!");

			var folderDialog = new OpenFolderDialog
			{

			};

			if(folderDialog.ShowDialog() == true)
			{
				_outputFolderPath = folderDialog.FolderName;
				OutputPath = _outputFolderPath;
				OnPropertyChanged(nameof(OutputPath));
			}
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
