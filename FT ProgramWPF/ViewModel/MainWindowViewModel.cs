using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FT_ProgramWPF.View;

namespace FT_ProgramWPF.ViewModel
{
	public class MainWindowViewModel : ObservableObject
	{
		public Page CurrentPage { get; set; }


		#region ViewModels

		private ConnectionSetupViewModel _connectionSetupViewModel;

		#endregion

		#region View

		private DownloadingView _DownloadingView;

		#endregion
		public MainWindowViewModel()
		{
			_connectionSetupViewModel = new ConnectionSetupViewModel();


			_connectionSetupViewModel.DisplayDownloadPage = ChangeDisplayDownloadPage;

			CurrentPage = new ConnectionSetupView(_connectionSetupViewModel);

		}

		public void ChangeDisplayDownloadPage()
		{
			if (_DownloadingView == null) 
			{
				_DownloadingView = new DownloadingView();
			}
			CurrentPage = _DownloadingView;

			OnPropertyChanged(nameof(CurrentPage));
		}

	}
}
