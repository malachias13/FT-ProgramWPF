using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FT_ProgramWPF.Managers;
using FT_ProgramWPF.View;

namespace FT_ProgramWPF.ViewModel
{
	public class MainWindowViewModel : ObservableObject
	{
		public Page CurrentPage { get; set; }

		private string _serverIP;
		private string _clientOutputFolderPath;

		private RpcClient _rpcClient;

		#region ViewModels

		private ConnectionSetupViewModel _connectionSetupViewModel;
		private DownloadingViewModel _downloadingViewModel;

		#endregion

		#region View

		private DownloadingView _DownloadingView;

		#endregion
		public MainWindowViewModel()
		{
			_connectionSetupViewModel = new ConnectionSetupViewModel();
			_downloadingViewModel = new DownloadingViewModel();


			_connectionSetupViewModel.DisplayDownloadPage = ChangeDisplayDownloadPage;
			_connectionSetupViewModel.SetOutputFolder = SetClientOutputFolder;
			_connectionSetupViewModel.SetIpAddress = SetServerIP;

			CurrentPage = new ConnectionSetupView(_connectionSetupViewModel);

		}

		public void ChangeDisplayDownloadPage()
		{
			if (_DownloadingView == null) 
			{
				_DownloadingView = new DownloadingView(_downloadingViewModel);
			}
			CurrentPage = _DownloadingView;

			OnPropertyChanged(nameof(CurrentPage));

			CreateClient();
		}

		private void SetServerIP(string ip)
		{
			_serverIP = ip;
		}

		private void SetClientOutputFolder(string folder)
		{
			_clientOutputFolderPath = folder;
		}

		private void CreateClient()
		{
			_rpcClient = new RpcClient(_serverIP);
		}

	}
}
