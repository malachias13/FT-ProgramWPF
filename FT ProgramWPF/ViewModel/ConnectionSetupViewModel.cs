using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FT_ProgramWPF.Managers;

namespace FT_ProgramWPF.ViewModel
{
	public partial class ConnectionSetupViewModel : ObservableObject
	{

		public ConnectionSetupViewModel()
		{
			ErrorMessageVisiblity = Visibility.Collapsed;
			HostButtonEnable = true;
			OnPropertyChanged(nameof(ErrorMessageVisiblity));
		}

		// Test server code
		private RpcServer server;
		private string debugStr;

		public string DebugStr { get; set; }
		// End test server code

		private string _outputFolderPath;
		private string _IPAddressStr;
		public Action DisplayDownloadPage;
		public Action<string> SetOutputFolder;
		public Action<string> SetIpAddress;

		bool HasURL = false;

		public bool HostButtonEnable {  get; set; }
		public Visibility ErrorMessageVisiblity { get; set; }
		public string OutputPath {  get; set; }
		public string IPAddressStr 
		{
			get { return _IPAddressStr; }
			set 
			{ 
				_IPAddressStr = value;
				validateIP();
				OnPropertyChanged(nameof(IPAddressStr)); 
			}
		}

		private void validateIP()
		{

			if (IPAddressStr.Length < 8)
			{
				HasURL = false;
				ErrorMessageVisiblity = Visibility.Collapsed;
				return;
			}

			string Header = IPAddressStr.Substring(0,8);

			if(Header == "https://")
			{
				HasURL = true;
				ErrorMessageVisiblity = Visibility.Collapsed;
				OnPropertyChanged(nameof(ErrorMessageVisiblity));
			}
			else
			{
				ErrorMessageVisiblity = Visibility.Visible;
				OnPropertyChanged(nameof(ErrorMessageVisiblity));
			}

		}

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

			HostButtonEnable = false;
			OnPropertyChanged(nameof(HostButtonEnable));


			try
			{
				// Test server code!
				server = new RpcServer(_outputFolderPath);

				await Task.Run(() => server.StartServer());
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				debugStr = ex.ToString();

				DebugStr = debugStr;
			}

		}

		[RelayCommand]
		async void Join()
		{

			if(!HasURL) 
			{
				ErrorMessageVisiblity = Visibility.Visible;
				OnPropertyChanged(nameof(ErrorMessageVisiblity));
				return; 
			}

			// Check to see if user set and Output folder.

			SetOutputFolder.Invoke(_outputFolderPath);
			SetIpAddress.Invoke(IPAddressStr);
			await Application.Current.Dispatcher.InvokeAsync(DisplayDownloadPage);
			Debug.WriteLine("Join!");
		}

	}
}
