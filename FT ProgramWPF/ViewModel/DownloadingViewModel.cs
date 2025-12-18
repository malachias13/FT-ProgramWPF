using CommunityToolkit.Mvvm.ComponentModel;
using FT_ProgramWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_ProgramWPF.ViewModel
{
	public partial class DownloadingViewModel : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<FileModel> files;

		public DownloadingViewModel()
		{
			files = new ObservableCollection<FileModel>();
		}

		public void AddFile(FileModel file)
		{
			files.Add(file);
		}

	}
}
