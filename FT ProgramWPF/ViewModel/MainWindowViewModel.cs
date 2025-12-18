using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FT_ProgramWPF.Managers;
using FT_ProgramWPF.Model;
using FT_ProgramWPF.View;
using Grpc.Core;
using RpcShared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FT_ProgramWPF.ViewModel
{
	public class MainWindowViewModel : ObservableObject
	{
		public Page CurrentPage { get; set; }

		public float CurrentProgress
		{
			get { return _currentProgress; }
			private set{ _currentProgress = value; OnPropertyChanged(); }
		}

		public Visibility ProgressBarIsVisible 
		{ 
			get { return _progressBarIsVisible; }
			private set { _progressBarIsVisible = value; OnPropertyChanged(); } 
		}

		private string _serverIP;
		private string _clientOutputFolderPath;
		private float _currentProgress;
		private Visibility _progressBarIsVisible;
		private float _progressBarDelay = 0.5f;

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

			_downloadingViewModel.GetServerFilesCommand = 
				new RelayCommand<DownloadingViewModel>(execute => RequestAndLoadServerFilesMeta());

			CurrentPage = new ConnectionSetupView(_connectionSetupViewModel);
			ProgressBarIsVisible = Visibility.Hidden;
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

			RequestAndLoadServerFilesMeta();
		}

		private async Task RequestAndLoadServerFilesMeta()
		{
			if (_rpcClient == null) { return; }

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

		private void ResetProgressBar(bool NewVisible)
		{
			ProgressBarIsVisible = Visibility.Hidden;
			CurrentProgress = 0;

			ProgressBarIsVisible = NewVisible == true ? Visibility.Visible : Visibility.Hidden;
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
	}
}
