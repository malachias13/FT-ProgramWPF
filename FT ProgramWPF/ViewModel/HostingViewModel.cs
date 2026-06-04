using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FT_ProgramWPF.Managers;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FT_ProgramWPF.ViewModel
{
	public class LogEntry
	{
		public string Message { get; set; }
		public string Color { get; set; } // e.g., "Red", "#FF0000"
	}

	public partial class HostingViewModel : ObservableObject
	{
		public ObservableCollection<string> LogEntries { get; } = new ObservableCollection<string>();
		public string ServerToggleTxt 
		{
			get { return serverToggleTxt; } 
			private set { serverToggleTxt = value; OnPropertyChanged(nameof(serverToggleTxt)); }
		}

		public string ServerToggleBtnColor {  get; set; }

		private RpcServer server;
		private string serverToggleTxt;
		private string severPath;

		public HostingViewModel()
		{

			//LogEntries.CollectionChanged += LogEntries_CollectionChanged;
			//Application.Current.Dispatcher.Invoke(async () =>
			//{

			//	for (int i = 0; i < 100; i++)
			//	{
			//		AddLog($"{i}: Hello ffkmr rfkrkmfkmr krkf kr kf kr kf kr " +
			//			$"rkflmrmfmmeomofm4ompofmmfomopm emfopmewpofmopmw fowmpofm wmfwmpo" +
			//			$"wofmwkrefm fowmpfmwpf wmfpwm fjwmporm wmfpwmmrw" +
			//			$"wormpwmef wekrpkmrfmwplefmwepokrr. END!");

			//		await Task.Delay(300);
			//	}
			//});
			ServerToggleBtnColor = "#1111";
		}


		public void OnDisplay(string outputPath = "")
		{

			severPath = outputPath;
			server = new RpcServer(severPath);

			server.OnPrintServerMessage += printServerLogs;
			server.OnStart += Start;
			server.OnStop += Stop;

			try
			{
				Task.Run(() =>
				{
					server.StartServer();
				});

			}
			catch (Exception ex)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					AddLog($"{ex.ToString()}");
				});
			}
		}


		private void printServerLogs(string message)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				AddLog($"{message}");
			});
		}

		private void Start()
		{
			serverToggleTxt = "Disconnect";
			ServerToggleBtnColor = "Red";

			OnPropertyChanged(nameof(serverToggleTxt));
			OnPropertyChanged(nameof(ServerToggleBtnColor));

		}

		private void Stop()
		{
			serverToggleTxt = "Connect";
			ServerToggleBtnColor = "#3c581c";

			OnPropertyChanged(nameof(serverToggleTxt));
			OnPropertyChanged(nameof(ServerToggleBtnColor));
		}
		


		public void AddLog(string message)
		{
			LogEntries.Add($"[{System.DateTime.Now:T}] {message}");
		}

		//public void AddLog(string msg, string color = "Black")
		//{
		//	LogEntries.Add(new LogEntry { Message = msg, Color = color });
		//}

		// COMMANDS
		[RelayCommand]
		async void ServerToggle()
		{

			if (null != server.GetServerStatusTracker() && server.GetServerStatusTracker().IsRunning)
			{
				server.StopServer();
			}
			else
			{
				LogEntries.Clear();
				try
				{
					await Task.Run(() =>
					{
						server.StartServer();
					});
				}
				catch
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						AddLog("server Error!");
					});
				}
			}
		}

	}
}

