/// <summary>
/// 
/// </summary>
/// 
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace ChatTest
{
    public class Program
    {
        static int Main(string[] arguments)
        {
            if (arguments.Length > 0)
            {
                if (arguments[0] == "server")
                {
                    Console.WriteLine(">> Running Server");
                    Server server = new Server();
                    server.Listen(IPAddress.Any.ToString(), 9001);
                }

                else if (arguments.Length >= 3 && arguments[0] == "client")
                {
                    Client client = new Client();

                    client.Run(arguments[1], Int32.Parse(arguments[2]));
                }
            }
            return 0;
        }
    }

    public class Server
    {
        public Server()
        {
        }

        public void Listen(string hostname, int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(hostname), port);
            listener.Start();
            Console.WriteLine($"Server is listening on {hostname}:{port}");

			int numOfMessages = 0;

            while (true)
            {
                using (TcpClient client = listener.AcceptTcpClient())
                {
                    byte[] buffer = new byte[2048];
                    int bytesRead;

                    using (NetworkStream stream = client.GetStream())
                    {
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
							numOfMessages++;
                            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            Console.WriteLine(numOfMessages.ToString() + " Data received: " + data);
                        }
                    }
                }
            }
        }
    }

    public class Client
    {
        TcpClient? client;
        public Client()
        {
            client = null;
        }

        public void Run(string hostname, int port)
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine(">> Write a message");
                Console.Write(">> ");
                string? message = Console.ReadLine();

                if (message != null)
                {
                    Connect(hostname, port, message);
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

                if (client != null)
                    stream = client.GetStream();

                if (stream != null)
				{
                    stream.Write(buffer, 0, buffer.Length);
					Console.WriteLine(">> Sent: " + message);
				}
            }

            catch (SocketException exception)
            {
                Console.WriteLine(exception);
            }

            finally
            {
                
                client?.Close();
            }
        }
    }
}
