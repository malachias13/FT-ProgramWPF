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
			HostButtonEnable = false;
			JoinButtonEnable = false;
			OnPropertyChanged(nameof(ErrorMessageVisiblity));
		}

		private string _outputFolderPath;
		private string _IPAddressStr;
		public Action DisplayDownloadPage;
		public Action DisplayHostingPage;
		public Action<string> SetOutputFolder;
		public Action<string> SetIpAddress;

		bool HasURL = false;

		public bool HostButtonEnable {  get; set; }
		public bool JoinButtonEnable { get; set; }
		public Visibility ErrorMessageVisiblity { get; set; }
		public string OutputPath 
		{
			get { return _outputFolderPath; }
			set 
			{
				_outputFolderPath = value;
				validateOutputPath();
			}
		}
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

		public void OnDisplay()
		{
			OutputPath = string.Empty;
			IPAddressStr = string.Empty;

			OnPropertyChanged(nameof(OutputPath));
			OnPropertyChanged(nameof(IPAddressStr));

			ErrorMessageVisiblity = Visibility.Collapsed;
			HostButtonEnable = false;
			JoinButtonEnable = false;

			OnPropertyChanged(nameof(ErrorMessageVisiblity));
			OnPropertyChanged(nameof(HostButtonEnable));
			OnPropertyChanged(nameof(JoinButtonEnable));

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

		private bool validateOutputPath()
		{

			bool enableButtons = false;
			if(_outputFolderPath != null)
			{
				enableButtons = true;
			}

			HostButtonEnable = enableButtons;
			JoinButtonEnable = enableButtons;

			OnPropertyChanged(nameof(HostButtonEnable));
			OnPropertyChanged(nameof(JoinButtonEnable));

			return enableButtons;
		}

		[RelayCommand]
		async void PickFolder()
		{
			var folderDialog = new OpenFolderDialog
			{

			};

			if(folderDialog.ShowDialog() == true)
			{
				OutputPath = folderDialog.FolderName;
				SetOutputFolder.Invoke(OutputPath);
				OnPropertyChanged(nameof(OutputPath));
			}
		}

		[RelayCommand]
		async void Host()
		{

			HostButtonEnable = false;
			OnPropertyChanged(nameof(HostButtonEnable));

			await Application.Current.Dispatcher.InvokeAsync(DisplayHostingPage);

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

			SetIpAddress.Invoke(IPAddressStr);
			await Application.Current.Dispatcher.InvokeAsync(DisplayDownloadPage);
		}

	}
}
