using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Browser
{
	public class SttpClient
	{
		public string ip;
		public int port;
		public string request;

		public SttpClient(string ip = "127.0.0.1", int port = 80)
		{
			this.ip = ip;
			this.port = port;
		}

		public void GenerateHeader(string path, string browser)
		{
			this.request = $"STTP {path}\nBrowser: {browser}\n";
			Console.WriteLine(this.request);
		}

		public void AddHeader(string text)
		{
			request += text + "\n";
		}

		public void Rebind(string ip, int port)
		{
			this.ip = ip;
			this.port = port;
		}

		public byte[] Get()
		{
			using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				try
				{
					client.Connect(ip, port);
				}
				catch (SocketException ex)
				{
					Console.WriteLine($"Socket connection error: {ex.Message}");
					return null;
				}

				if (!client.Connected)
				{
					Console.WriteLine("Unable to connect to the server.");
					return null;
				}
				client.Send(Encoding.UTF8.GetBytes(request));

				client.ReceiveTimeout = 4000; // Установим тайм-аут для получения данных

				byte[] buffer = new byte[1024 * 64];
				int totalBytes = 0;
				int receivedBytes;

				try
				{
					while ((receivedBytes = client.Receive(buffer, totalBytes, buffer.Length - totalBytes, SocketFlags.None)) > 0)
					{
						totalBytes += receivedBytes;
					}
				}
				catch (SocketException ex)
				{
					if (ex.SocketErrorCode != SocketError.TimedOut)
					{
						Console.WriteLine($"Socket receive error: {ex.Message}");
					}
				}

				client.Shutdown(SocketShutdown.Both);
				client.Close();

				return buffer.Take(totalBytes).ToArray();
			}
		}

		public string Send()
		{
			return Encoding.UTF8.GetString(Get());
		}
	}
}
