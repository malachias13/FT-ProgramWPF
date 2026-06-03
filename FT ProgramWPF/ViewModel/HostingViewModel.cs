using CommunityToolkit.Mvvm.ComponentModel;
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

		public RichTextBox RichTextBoxCustom { get; set; }

		private RpcServer server;

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

		}


		public void OnDisplay()
		{
			// Test code!
			server = new RpcServer("C:\\Users\\malac\\Desktop\\Output\\ServerFiles");

			server.OnPrintServerMessage += printServerLogs;
			try
			{
				Task.Run(() =>
				{
					server.StartServer();
				});

				Application.Current.Dispatcher.Invoke(() =>
				{
					AddLog($"END Message! CUSTOM");
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


		public void printServerLogs(string message)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				AddLog($"{message}");
			});
		}


		public void AddLog(string message)
		{
			LogEntries.Add($"[{System.DateTime.Now:T}] {message}");
		}

		//public void AddLog(string msg, string color = "Black")
		//{
		//	LogEntries.Add(new LogEntry { Message = msg, Color = color });
		//}

		// TEst code
		private void LogEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (LogEntry entry in e.NewItems)
				{
					AppendLogEntry(entry);
				}

				// Auto-scroll to bottom
				RichTextBoxCustom.ScrollToEnd();
			}
		}

		private void AppendLogEntry(LogEntry entry)
		{
			var paragraph = new Paragraph();
			var run = new Run(entry.Message + Environment.NewLine);

			// Apply formatting
			if (!string.IsNullOrEmpty(entry.Color))
			{
				try { run.Foreground = (Brush)new BrushConverter().ConvertFromString(entry.Color); }
				catch { /* Handle invalid color */ }
			}

			paragraph.Inlines.Add(run);
			RichTextBoxCustom.Document.Blocks.Add(paragraph);
		}

		//private void LogEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		//{
		//	if (e.Action == NotifyCollectionChangedAction.Add)
		//	{
		//		// Use Dispatcher to ensure UI is updated before scrolling
		//		Application.Current.Dispatcher.InvokeAsync(() =>
		//		{
		//			LogScrollViewer.ScrollToEnd();
		//		}, System.Windows.Threading.DispatcherPriority.Loaded);
		//	}
		//}
	}
}

