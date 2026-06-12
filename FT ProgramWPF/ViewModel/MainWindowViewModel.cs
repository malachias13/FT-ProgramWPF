using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FT_ProgramWPF.Managers;
using FT_ProgramWPF.Model;
using FT_ProgramWPF.View;
using Grpc.Core;
using RpcShared;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FT_ProgramWPF.ViewModel
{
	public class MainWindowViewModel : ObservableObject
	{
		public Page CurrentPage { get; set; }

		public float CurrentProgress
		{
			get { return _currentProgress; }
			private set { _currentProgress = value; OnPropertyChanged(); }
		}

		public Visibility ProgressBarIsVisible
		{
			get { return _progressBarIsVisible; }
			private set { _progressBarIsVisible = value; OnPropertyChanged(); }
		}

		public string WindowsDisplayData
		{
			get { return _windowsDisplayData; }
			set { _windowsDisplayData = value; OnPropertyChanged(); }
		}
		public Brush WindowsDisplayForeground
		{
			get { return _windowsDisplayForeground; }
			set { _windowsDisplayForeground = value; OnPropertyChanged(); }
		}

		public string WindowDisplayVersion
		{
			get { return _windowDisplayVersion; }
			set { _windowDisplayVersion = value; OnPropertyChanged(); }
		}

		private string _serverIP;
		private string _clientOutputFolderPath;
		private float _currentProgress;
		private Visibility _progressBarIsVisible;
		private float _progressBarDelay = 0.5f;

		private string _windowsDisplayData;
		private string _windowDisplayVersion;
		private Brush _windowsDisplayForeground;

		private UpdateManager _updateManager;

		private RpcClient _rpcClient;

		#region ViewModels

		private ConnectionSetupViewModel _connectionSetupViewModel;
		private DownloadingViewModel _downloadingViewModel;
		private HostingViewModel _hostingViewModel;

		#endregion

		#region View

		private ConnectionSetupView _connectionSetupView;
		private DownloadingView _DownloadingView;
		private HostingView _HostingView;

		#endregion
		public MainWindowViewModel()
		{
			_connectionSetupViewModel = new ConnectionSetupViewModel();
			_downloadingViewModel = new DownloadingViewModel();
			_hostingViewModel = new HostingViewModel();


			_connectionSetupViewModel.DisplayDownloadPage = ChangeDisplayDownloadPage;
			_connectionSetupViewModel.SetOutputFolder = SetClientOutputFolder;
			_connectionSetupViewModel.SetIpAddress = SetServerIP;
			_connectionSetupViewModel.DisplayHostingPage = ChangeDisplayHostingPage;

			_hostingViewModel.OnReturnToMainPage = ReturnToMainMenu;

			_downloadingViewModel.GetServerFilesCommand =
				new RelayCommand<DownloadingViewModel>(execute => RequestAndLoadServerFilesMeta());

			_downloadingViewModel.SeverToggleCommand =
				new RelayCommand<DownloadingViewModel>(execute => ClientConnectionToggle());

			_downloadingViewModel.ReturnToMainPageCommand =
				new RelayCommand<DownloadingViewModel>(execute => ReturnToMainMenu());

			_connectionSetupView = new ConnectionSetupView(_connectionSetupViewModel);

			CurrentPage = _connectionSetupView;
			ProgressBarIsVisible = Visibility.Hidden;
			WindowsDisplayForeground = Brushes.White;
		}

		public async void MainWindowLoaded(object sender, RoutedEventArgs e)
		{

			try
			{
				_updateManager = await UpdateManager
					.GitHubUpdateManager(@"https://github.com/malachias13/FT-ProgramWPF");
				string text = $"FT Program {_updateManager.CurrentlyInstalledVersion()} created by Malachias Harris";
				WindowDisplayVersion = text;
				CheckForUpdates();
			}
			catch
			{
				string text = "FT Program created by Malachias Harris";
				WindowDisplayVersion = text;
			}
		}

		private async void CheckForUpdates()
		{
			try
			{
				ResetProgressBar(true);
				SetIsCheckingForUpdates(true);
				var UpdateInfo = await _updateManager.CheckForUpdate(false, UpdateProgressBar);
				if (UpdateInfo.ReleasesToApply.Count  > 0)
				{
					// Notify the user.
					Update();
				}
				await Task.Delay(1000).ContinueWith(tt =>
				{
					UpdateProgressBar(0);
					SetIsCheckingForUpdates(false);
					ResetProgressBar(false);
				});
			}
			catch
			{
				WindowsDisplayForeground = Brushes.Red;
				WindowsDisplayData = "Failed to update...";
				await Task.Delay(3000).ContinueWith(tt => SetIsCheckingForUpdates(false));
			}
		}

		private void Update()
		{
			Task.Run(() => _updateManager.UpdateApp(UpdateProgressBar)).GetAwaiter().OnCompleted(() =>
			{

				UpdateProgressBar(0);
				ResetProgressBar(false);
				string text = $"File Gambit {_updateManager.CurrentlyInstalledVersion()} created by Malachias Harris";
				WindowDisplayVersion = text;
			});

		}

		private void SetIsCheckingForUpdates(bool isUpdating)
		{
			if (isUpdating)
			{
				WindowsDisplayForeground = WindowsDisplayForeground = Brushes.LimeGreen;
				WindowsDisplayData = "Checking for updates...";
			}
			else
			{
				WindowsDisplayData = "";
			}
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

		public void ChangeDisplayHostingPage()
		{
			if (_HostingView == null)
			{
				_HostingView = new HostingView(_hostingViewModel);
			}
			CurrentPage = _HostingView;
			OnPropertyChanged(nameof(CurrentPage));

			_hostingViewModel.OnDisplay(_clientOutputFolderPath);

		}

		private void SetServerIP(string ip)
		{
			_serverIP = ip;
		}

		private void SetClientOutputFolder(string folder)
		{
			_clientOutputFolderPath = folder;
		}

		private void ReturnToMainMenu()
		{
			CurrentPage = _connectionSetupView;
			_connectionSetupViewModel.OnDisplay();
			OnPropertyChanged(nameof(CurrentPage));
		}

		#region ClientCode
		private void CreateClient()
		{
			_rpcClient = new RpcClient(_serverIP);
			_rpcClient.OnConnectionReady += ClientConnected;
			_rpcClient.OnDisconnected += ClientDisconnected;
			_rpcClient.Connect();
		}

		private void ClientConnected()
		{
			RequestAndLoadServerFilesMeta();
			_downloadingViewModel.SetSeverBtnStyle(true);
			_downloadingViewModel.SetReturnToMainPageBtnEnable(false);
		}

		private void ClientDisconnected()
		{
			_downloadingViewModel.SetSeverBtnStyle(false);
			_downloadingViewModel.SetReturnToMainPageBtnEnable(true);
			_downloadingViewModel.ClearAllFiles();
		}

		private void ClientConnectionToggle()
		{
			if(_rpcClient?.IsConnected() == true)
			{
				_rpcClient?.Disconnect();
			}
			else
			{
				_downloadingViewModel.SetSeverBtnStyleConnecting();
				_rpcClient?.Connect();
			}
		}

		private async Task RequestAndLoadServerFilesMeta()
		{
			if (_rpcClient == null) { return; }

			// Validation
			if (_rpcClient.IsConnected() == false)
			{
				_downloadingViewModel.ClearAllFiles();
				return;
			}

			_downloadingViewModel.ClearAllFiles();

			FileMetadataReply result = null;

			result = await _rpcClient.GetAllFilesMetadataAsync();

			ResetProgressBar(true);
			int resutCount = result.FilesInfo.Count;
			int current = 0;

			foreach (var file in result.FilesInfo)
			{
				DateTime dateTime = DateTime.Parse($"{file.Date} {file.Time}");

				FileModel model = new FileModel
				{
					Name = file.Filename,
					Data = dateTime
				};

				Client_FileDisplayAdd(model);
				current++;
				UpdateProgressBar(current, resutCount);
			}

			await Task.Delay(TimeSpan.FromSeconds(_progressBarDelay));

			ResetProgressBar(false);
		}

		private void Client_FileDisplayAdd(FileModel model)
		{
			model.DownloadCommand = new RelayCommand<FileModel>(execute => Download(model));
			_downloadingViewModel.AddFile(model);
		}

		private async Task Download(FileModel model)
		{
			// Validation
			if (_rpcClient.IsConnected() == false)
			{
				_downloadingViewModel.ClearAllFiles();
				return;
			}

			var call = _rpcClient.GetFile(model.Name);
			ResetProgressBar(true);
			long maxSize = 0;
			long current = 0;

			if (_clientOutputFolderPath != null)
			{

				Stream fs = File.OpenWrite($"{_clientOutputFolderPath}\\{model.Name}");

				await foreach (var chunk in call.ResponseStream.ReadAllAsync().ConfigureAwait(false))
				{
					maxSize = call.ResponseStream.Current.FileSize;

					current += chunk.ChunkSize;

					UpdateProgressBar(current, maxSize);

					fs.Write(chunk.Data.ToByteArray());
				}

				fs.Close();
			}

			await Task.Delay(TimeSpan.FromSeconds(_progressBarDelay));

			ResetProgressBar(false);

			Debug.WriteLine($"{model.Name} is DONE!");
		}
		#endregion

		#region ProgressBar

		private void ResetProgressBar(bool NewVisible)
		{
			ProgressBarIsVisible = Visibility.Hidden;
			CurrentProgress = 0;

			ProgressBarIsVisible = NewVisible == true ? Visibility.Visible : Visibility.Hidden;
		}

		private void UpdateProgressBar(int current)
		{
			CurrentProgress = current;
		}

		private void UpdateProgressBar(int current, int max)
		{
			float result = (float)current / (float)max;
			CurrentProgress = result;
		}

		private void UpdateProgressBar(long current, long max)
		{
			float result = (float)current / (float)max;
			CurrentProgress = result;
		}
		#endregion
	}
}