using FT_ProgramWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FT_ProgramWPF.View
{
	/// <summary>
	/// Interaction logic for ConnectionSetupView.xaml
	/// </summary>
	public partial class ConnectionSetupView : Page
	{
		public ConnectionSetupView(ConnectionSetupViewModel vm)
		{
			InitializeComponent();

			DataContext = vm;
		}
	}
}
