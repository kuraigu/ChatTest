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
                    //server.Listen(IPAddress.Loopback.ToString(), 9001);
					server.Test();
                }

                else if (arguments[0] == "client")
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
        public Server()
        {
        }

        public void Test()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    using TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public void Listen(string hostname, int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(hostname), port);
            listener.Start();
            Console.WriteLine($"Server is listening on {hostname}:{port}");

            while (true)
            {
                using (TcpClient client = listener.AcceptTcpClient())
                {
                    Console.WriteLine(">> Connected to client");

                    byte[] buffer = new byte[2048];
                    int bytesRead;

                    using (NetworkStream stream = client.GetStream())
                    {
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            Console.WriteLine("Data received: " + data);
                        }
                    }

                    Console.WriteLine(">> Client disconnected");
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

        public void Run()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine(">> Write a message");
                Console.Write(">> ");
                string? message = Console.ReadLine();

                if (message != null)
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

                if (client != null)
                    stream = client.GetStream();

                if (stream != null)
                    stream.Write(buffer, 0, buffer.Length);
            }

            catch (SocketException exception)
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
