using System.Net;
using System.Net.Sockets;

public static class Server
{
    private static byte[] buffer = new byte[512 * 1024];

    private static void OnSocketSend(IAsyncResult ar)
    {
        Socket client = (Socket)ar.AsyncState;
        try
        {
            // Complete the send operation
            int bytesSent = client.EndSend(ar);
            Console.WriteLine($"Sent {bytesSent} bytes to the client.");

            // Instead of recursive call, check if the client is still connected and send again
            if (client.Connected)
            {
                // Use a delay or control flow here if needed
                Thread.Sleep(100);
                client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, OnSocketSend, client);
            }
            else
            {
                Console.WriteLine("Client disconnected.");
                client.Close();
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Socket exception: {ex.Message}");
            client.Close();
        }
        catch (ObjectDisposedException)
        {
            Console.WriteLine("Socket closed.");
        }
    }

    public static void Main(string[] args)
    {
        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(new IPEndPoint(IPAddress.Loopback, 5555));
        server.Listen(10);

        Console.WriteLine("Server started, waiting for connections...");

        while (true)
        {
            // Accept client connection
            Socket client = server.Accept();
            Console.WriteLine("Client connected.");

            // Start sending data to the client
            client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, OnSocketSend, client);
        }
    }
}
