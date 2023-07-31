/// <summary>
/// 
/// </summary>
/// 
using System;
using System.Net.Sockets;
using System.Text;
namespace ChatTest
{
    public class Program
    {
		static int Main(string[] arguments)
		{
			if(arguments.Length > 0)
			{
				if(arguments[0] == "server")
				{
					Console.WriteLine(">> Running Server");
					Server server = new Server();
					server.Listen("127.0.0.1", 9001);
				}

				else if(arguments[0] == "client")
				{
					Client client = new Client();

					client.Run();
				}
			}
			return 0;
		}
    }

	public class Server 
	{
		private TcpListener? listener;
		private TcpClient? client;
		
		public Server()
		{
			listener = null;
			client = null;
		}

		public int Listen(string hostname, int port)
		{
			try
			{
				listener = new TcpListener(System.Net.IPAddress.Parse(hostname), port);

				listener.Start();

				Byte[] buffer = new Byte[2048];
				string? data;

				bool listening = true;
				while(listening)
				{
					Console.WriteLine(">> Waiting for connection");
					
					client = listener.AcceptTcpClient();
					NetworkStream stream = client.GetStream();
					data = null; 
					Console.WriteLine(">> Connected");

					while(listening)
					{
						int i = stream.Read(buffer, 0, buffer.Length);
						if(i == 0) break;

						data = Encoding.ASCII.GetString(buffer, 0, i);

						Console.WriteLine("Data: " + data);
					}
				}
			}

			catch (SocketException exception)
			{
				Console.WriteLine(exception);
			}

			finally
			{
				if(listener != null) listener.Stop();
			}

			return 0;
		}		
	}

	public class Client 
	{
		TcpClient? client;
		public Client()
		{
			client = null;
		}

		public void Run()
		{
			bool running = true;

			while(running)
			{
				Console.WriteLine(">> Write a message");
				Console.Write(">> ");
				string? message = Console.ReadLine();

				if(message != null)
				{
					Connect("192.168.254.140", 9001, message);
				}
			}
		}

		public void Connect(string hostname, int port, string message)
		{
			try
			{
				client = new TcpClient();
				client?.Connect(hostname, port);

				NetworkStream? stream = null;

				Byte[] buffer = Encoding.ASCII.GetBytes(message);

				if(client != null)
					stream = client.GetStream();

				if(stream != null)
					stream.Write(buffer, 0, buffer.Length);
			}

			catch(SocketException exception)
			{
				Console.WriteLine(exception);
			}

			finally 
			{
				Console.WriteLine(">> Sent: " + message);
				client?.Close();
			}
		}
	}
}
