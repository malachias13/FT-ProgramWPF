using System;
using System.Collections.Generic;
using System.Linq;
using FT_ProgramWPF.Managers;
using System.Net.Http;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using RpcShared;
using System.Text;
using System.Threading.Tasks;

namespace FT_ProgramWPF.Managers
{
	public class RpcClient
	{

		GrpcChannel channel;
		TPL.TPLClient TPLClient;

		public RpcClient(string serverAddress)
		{

			//var cert = FileCollection.CreateX509Certificate2("client-Cert");

			var baseUri = new Uri(serverAddress);

			var httpClientHandler = new HttpClientHandler();
			// httpClientHandler.ClientCertificates.Add(cert);
			httpClientHandler.ServerCertificateCustomValidationCallback =
				HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

			channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
			{
				HttpHandler = new GrpcWebHandler(httpClientHandler)
			});

			TPLClient = new TPL.TPLClient(channel);

			TPLClient.SayHelloAsync(new HelloRequest { Name = "Android client!" });
		}

		// TODO Make functions to get files from the server.


		// TODO Get All Files info {Name, Date, maybe thumbnil}

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



		// ToDO Get File by name {download}
		public AsyncServerStreamingCall<FileReplay> GetFile(string filename)
		{
			var input = new GetFileRequest { Filename = filename };
			var reply = TPLClient.GetFile(input);

			return reply;
		}

		// Could send file to server but I may just make it readonly.
	}
}
