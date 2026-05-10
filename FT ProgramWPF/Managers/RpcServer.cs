using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpcShared;
using RPCServerFT.Services;
using Microsoft.Extensions.Logging;

namespace FT_ProgramWPF.Managers
{
	public class RpcServer
	{
		private string serverPath;
		private WebApplication app;
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
				return new TPLService(logger, serverfolderPath);
			});

			app = builder.Build();

			app.UseGrpcWeb();
			app.UseCors("AllowAll");
			app.MapGrpcService<TPLService>().EnableGrpcWeb();

			app.Run();
		}

		public void StopServer()
		{
			app?.StartAsync();
		}

	}
}
