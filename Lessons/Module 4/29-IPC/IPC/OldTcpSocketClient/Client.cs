using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

public static class Client
{
    private static byte[] buffer = new byte[512 * 1024];
    private static Stopwatch watch = new Stopwatch();
    private static long traffic = 0;
    private static int step = 0;

    private static void OnSocketReceive(IAsyncResult ar)
    {
        Socket client = (Socket)ar.AsyncState;
        try
        {
            // Complete the receive operation
            int bytesReceived = client.EndReceive(ar);
            if (bytesReceived > 0)
            {
                // Update traffic and steps
                traffic += bytesReceived;
                step++;

                // Log throughput every 1000 steps
                if ((step % 10) == 0)
                {
                    watch.Stop();
                    Console.WriteLine("{0} MB/s", (1000 * (traffic >> 20)) / watch.ElapsedMilliseconds);
                    watch.Start();
                }

                // Continue receiving data
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnSocketReceive, client);
            }
            else
            {
                // If no data is received, assume the server disconnected
                Console.WriteLine("Server disconnected.");
                client.Close();
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Socket exception: " + ex.Message);
            client.Close();
        }
        catch (ObjectDisposedException)
        {
            Console.WriteLine("Socket closed.");
        }
    }

    public static void Main(string[] args)
    {
        Thread.Sleep(2000); // Simulate delay

        try
        {
            // Create a new socket and connect to the server
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //client.ReceiveBufferSize = 2 * buffer.Length;
            client.Connect(new IPEndPoint(IPAddress.Loopback, 5555));

            Console.WriteLine("CONNECTED");

            // Start receiving data asynchronously
            watch.Start();
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnSocketReceive, client);

            Console.ReadLine(); // Keep the client running
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Error connecting to server: " + ex.Message);
        }
    }
}
