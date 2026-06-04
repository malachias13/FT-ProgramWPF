using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RPCServerFT.Services;
using RpcShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_ProgramWPF.Managers
{

	public class ServerStatusTracker
	{
		public bool IsRunning { get; private set; } = false;

		public ServerStatusTracker(IHostApplicationLifetime lifetime)
		{
			lifetime.ApplicationStarted.Register(() => IsRunning = true);
			lifetime.ApplicationStopping.Register(() => IsRunning = false);
			lifetime.ApplicationStopped.Register(() => IsRunning = false);
		}
	}


	public class RpcServer
	{
		private string serverPath;
		private WebApplication app;
		private string serverMessage;
		private ServerStatusTracker tracker;

		public Action<string> OnPrintServerMessage;

		public RpcServer(string _serverPath) 
		{
			serverPath = _serverPath;
		}

		public void StartServer()
		{
			var builder = WebApplication.CreateBuilder();

			builder.Services.AddGrpc();
			builder.Services.AddCors(o => o.AddPolicy("AllowAll", corsBuilder =>
			{
				corsBuilder.AllowAnyOrigin()
						   .AllowAnyMethod()
						   .AllowAnyHeader()
						   .WithExposedHeaders("Grpc-Status", "Grpc-Message",
											   "Grpc-Encoding", "Grpc-Accept-Encoding");
			}));

			builder.Services.AddSingleton<TPLService>(sp =>
			{
				var logger = sp.GetRequiredService<ILogger<TPLService>>();
				string serverfolderPath = serverPath;
				TPLService TPL = new TPLService(logger, serverfolderPath);
				TPL.OnPrintMessage += OnPrintServerMessage;
				return TPL;
			});

			builder.Services.AddSingleton<ServerStatusTracker>();

			app = builder.Build();

			app.UseGrpcWeb();
			app.UseCors("AllowAll");
			app.MapGrpcService<TPLService>().EnableGrpcWeb();


			app.Start();

			// Handle server status
			tracker = app.Services.GetService<ServerStatusTracker>();

			// Handle server messages!

			if (null != tracker && tracker.IsRunning)
			{
				string serverUrls = string.Join(", ", app.Urls);
				serverMessage = $"Now listening on: {serverUrls}";
				OnPrintServerMessage?.Invoke(serverMessage);
			}
			
			app.WaitForShutdown();
		}

		public void StopServer()
		{
			app?.StopAsync();

			if (null != tracker && !tracker.IsRunning)
			{
				OnPrintServerMessage?.Invoke($"Shutdown server");
			}
		}

		public ServerStatusTracker GetServerStatusTracker() { return tracker; }

		public string GetServerStartMessage()
		{
			return serverMessage;
		}

	}
}
