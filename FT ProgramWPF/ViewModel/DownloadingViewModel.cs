using CommunityToolkit.Mvvm.ComponentModel;
using FT_ProgramWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FT_ProgramWPF.ViewModel
{
	public partial class DownloadingViewModel : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<FileModel> files;

		public ICommand GetServerFilesCommand {  get; set; }
		public ICommand SeverToggleCommand { get; set; }
		public string ServerToggleBtnColor { get; private set; }
		public string ServerToggleTxt
		{
			get { return serverToggleTxt; }
			private set { serverToggleTxt = value; OnPropertyChanged(nameof(serverToggleTxt)); }
		}
		public bool ServerToggleBtnEnable { get; set; }

		private string serverToggleTxt;

		public DownloadingViewModel()
		{
			files = new ObservableCollection<FileModel>();

			ServerToggleBtnColor = "#1111";
			ServerToggleBtnEnable = true;
		}

		public void SetSeverBtnStyle(bool isConnected)
		{
			if(isConnected)
			{
				serverToggleTxt = "Disconnect";
				ServerToggleBtnColor = "Red";
			}
			else
			{
				serverToggleTxt = "Connect";
				ServerToggleBtnColor = "#3c581c";
			}
			ServerToggleBtnEnable = true;

			OnPropertyChanged(nameof(serverToggleTxt));
			OnPropertyChanged(nameof(ServerToggleBtnColor));
			OnPropertyChanged(nameof(ServerToggleBtnEnable));
		}

		public void SetSeverBtnStyleConnecting()
		{
			serverToggleTxt = "Connecting...";
			ServerToggleBtnEnable = false;

			OnPropertyChanged(nameof(serverToggleTxt));
			OnPropertyChanged(nameof(ServerToggleBtnEnable));
		}

		public void AddFile(FileModel file)
		{
			files.Add(file);
		}

		public void ClearAllFiles()
		{
			files.Clear();
		}

	}
}
