using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FT_ProgramWPF.Managers
{
	public class FileCollection
	{
		public List<FileInfo> files = new List<FileInfo>();

		public static byte[] ReadDataFromFile(string path)
		{
			return File.ReadAllBytes(path);
		}

		public static void WriteDataToFile(string path, byte[] data)
		{
			FileStream OutputFile = new FileStream(path, FileMode.Create);
			OutputFile.Write(data);
			OutputFile.Close();
		}

		public void OpenFolderAndGetFiles(string folder)
		{
			foreach (string file in Directory.EnumerateFiles(folder))
			{
				FileInfo fileInfo = new FileInfo(file);
				files.Add(fileInfo);
			}
		}

		public FileInfo? GetFile(string filename)
		{
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo.Name.Equals(filename))
				{
					return fileInfo;
				}
			}
			return null;
		}

		public static byte[] CreateX509Certificate2Bytes(string certName, string? password)
		{
			var ecdsa = ECDsa.Create();
			var rsa = RSA.Create();

			var req = new CertificateRequest($"cn={certName}", rsa,
				HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

			var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

			return cert.Export(X509ContentType.Pfx, password);
		}

		public static X509Certificate2 CreateX509Certificate2(string certName)
		{
			var ecdsa = ECDsa.Create();
			var rsa = RSA.Create();

			var req = new CertificateRequest($"cn={certName}", rsa,
				HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

			var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

			return cert;
		}
	}
}
