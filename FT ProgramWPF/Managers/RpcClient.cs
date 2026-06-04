using FT_ProgramWPF.Managers;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using RpcShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FT_ProgramWPF.Managers
{
	public class RpcClient
	{

		GrpcChannel channel;
		TPL.TPLClient TPLClient;
		Uri baseUri;

		public Action OnConnectionReady;
		public Action OnDisconnected;

		public RpcClient(string serverAddress)
		{
			baseUri = new Uri(serverAddress);
		}


		public async void Connect()
		{
			var httpClientHandler = new SocketsHttpHandler();
			httpClientHandler.SslOptions.RemoteCertificateValidationCallback =
				(sender, certificate, chain, sslPolicyErrors) => true;

			channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
			{
				HttpHandler = new GrpcWebHandler(httpClientHandler)
			});

			TPLClient = new TPL.TPLClient(channel);

			await WaitForReadyAndExecuteAsync(channel, async () =>
			{

				await TPLClient.SayHelloAsync(new HelloRequest { Name = "Desktop client!" });

				OnConnectionReady?.Invoke();
			});

			await channel.WaitForStateChangedAsync(ConnectivityState.Ready, CancellationToken.None);

			if (channel.State == ConnectivityState.Shutdown || channel.State == ConnectivityState.Idle)
			{
				OnDisconnected?.Invoke();
			}
		}

		public async void Disconnect()
		{

			await TPLClient.SayHelloAsync(new HelloRequest { Name = "Desktop client: Disconnected." });

			channel.Dispose();

			OnDisconnected?.Invoke();
		}

		private async Task WaitForReadyAndExecuteAsync(GrpcChannel channel, Func<Task> onReady)
		{
			// Loop until the channel is Ready or Shutdown
			while (channel.State != ConnectivityState.Ready && channel.State != ConnectivityState.Shutdown)
			{
				var currentState = channel.State;

				// If idle, force a connection attempt
				if (currentState == ConnectivityState.Idle)
				{
					await channel.ConnectAsync();
				}

				// Wait for the state to change from the current state
				// This returns immediately if the state has already changed
				await channel.WaitForStateChangedAsync(currentState, CancellationToken.None);
			}

			if (channel.State == ConnectivityState.Ready)
			{
				// Channel is Ready: Execute your function
				await onReady();
			}
			else
			{
				throw new InvalidOperationException($"Channel terminated in state: {channel.State}");
			}
		}

		public async Task<RpcShared.FileMetadataReply> GetAllFilesMetadataAsync()
		{
			var input = new GetAllFileMetadataRequest();
			var reply = await TPLClient.GetAllFilesMeatadataAsync(input);

			return reply;
		}

		public FileMetadataReply GetAllFilesMetadata()
		{
			var input = new GetAllFileMetadataRequest();
			var reply = TPLClient.GetAllFilesMeatadata(input);

			return reply;
		}

		public AsyncServerStreamingCall<FileReplay> GetFile(string filename)
		{
			var input = new GetFileRequest { Filename = filename };
			var reply = TPLClient.GetFile(input);

			return reply;
		}

		// Vaildation

		public bool IsConnected()
		{
			if(channel ==  null) return false;

			return channel.State == ConnectivityState.Ready;
		}


		// Could send file to server but I may just make it readonly.
	}
}
