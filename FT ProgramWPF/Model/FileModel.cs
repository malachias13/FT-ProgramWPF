using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FT_ProgramWPF.Model
{
	public class FileModel
	{
		public string? Name { get; set; }
		public DateTime Data { get; set; }
		public FileInfo? File { get; set; }
		public ICommand DownloadCommand { get; set; }
	}
}
