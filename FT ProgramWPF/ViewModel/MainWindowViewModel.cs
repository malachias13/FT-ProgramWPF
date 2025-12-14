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

		public MainWindowViewModel()
		{
			CurrentPage = new ConnectionSetupView();
		}
	}
}
