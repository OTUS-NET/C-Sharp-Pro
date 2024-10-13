using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Client
{
    private const string ServerAddress = "127.0.0.1";
    private const int Port = 8080;

    public static async Task StartClientAsync()
    {
        using (TcpClient client = new TcpClient())
        {
            await client.ConnectAsync(ServerAddress, Port);
            Console.WriteLine("Connected to the server!");

            NetworkStream stream = client.GetStream();
            string inputMessage = string.Empty;

            while (inputMessage != "exit")
            {
                Console.Write("Enter message to send (type 'exit' to disconnect): ");
                inputMessage = Console.ReadLine();
                if (string.IsNullOrEmpty(inputMessage))
                    continue;

                byte[] messageData = Encoding.UTF8.GetBytes(inputMessage);

                // Send message length first
                byte[] lengthPrefix = BitConverter.GetBytes(messageData.Length);
                await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);

                // Send the actual message
                await stream.WriteAsync(messageData, 0, messageData.Length);
                Console.WriteLine($"Sent to server: {inputMessage}");

                // Receive the echo from the server
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine($"Received from server: {response}");
            }
            Console.WriteLine("Client disconnected.");
        }
    }

    static async Task Main(string[] args)
    {
        using (EventWaitHandle eventHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\MyIPCEvent"))
        {
            eventHandle.WaitOne();
            await StartClientAsync();
        }
    }
}

