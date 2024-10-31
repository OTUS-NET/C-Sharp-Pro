using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    private const int Port = 8080;
    private const int BufferSize = 1024;
    private Socket listenerSocket;

    public void Start()
    {
        listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listenerSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
        listenerSocket.Listen(2);
        Console.WriteLine($"Server started on port {Port}...");

        StartAccept();
    }

    private void StartAccept()
    {
        SocketAsyncEventArgs acceptEventArgs = new SocketAsyncEventArgs();
        acceptEventArgs.Completed += AcceptCompleted;
        if (!listenerSocket.AcceptAsync(acceptEventArgs))
        {
            ProcessAccept(acceptEventArgs);
        }
    }

    private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
    {
        ProcessAccept(e);
    }

    private void ProcessAccept(SocketAsyncEventArgs e)
    {
        Socket clientSocket = e.AcceptSocket;
        Console.WriteLine("Client connected.");

        SocketAsyncEventArgs receiveEventArgs = new SocketAsyncEventArgs();
        receiveEventArgs.SetBuffer(new byte[BufferSize], 0, BufferSize);
        receiveEventArgs.AcceptSocket = clientSocket;
        receiveEventArgs.Completed += ReceiveCompleted;

        StartReceive(receiveEventArgs);

        StartAccept(); // Continue accepting other clients
    }

    private void StartReceive(SocketAsyncEventArgs receiveEventArgs)
    {
        if (!receiveEventArgs.AcceptSocket.ReceiveAsync(receiveEventArgs))
        {
            ProcessReceive(receiveEventArgs);
        }
    }

    private void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
    {
        ProcessReceive(e);
    }

    private void ProcessReceive(SocketAsyncEventArgs e)
    {
        Socket clientSocket = e.AcceptSocket;

        if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
        {
            // Read the message length first (assumes 4 bytes for an integer)
            int messageLength = BitConverter.ToInt32(e.Buffer, 0);
            Console.WriteLine($"Expected message length: {messageLength} bytes");

            // Prepare buffer to receive the actual message
            byte[] messageBuffer = new byte[messageLength];
            int totalBytesRead = 0;

            while (totalBytesRead < messageLength)
            {
                int bytesRead = clientSocket.Receive(messageBuffer, totalBytesRead, messageLength - totalBytesRead, SocketFlags.None);
                if (bytesRead == 0)
                {
                    // Client disconnected unexpectedly
                    break;
                }
                totalBytesRead += bytesRead;
            }

            // Convert message to string
            string receivedMessage = Encoding.UTF8.GetString(messageBuffer);
            Console.WriteLine($"Received: {receivedMessage}");

            // Echo the data back to the client
            byte[] echoData = Encoding.UTF8.GetBytes(receivedMessage);
            clientSocket.Send(echoData);
            Console.WriteLine("Echoed data back to client.");

            // Continue receiving more data from the client
            StartReceive(e);
        }
        else
        {
            Console.WriteLine("Client disconnected.");
            clientSocket.Close();
        }
    }

    static void Main(string[] args)
    {
        using (EventWaitHandle eventHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\MyIPCEvent"))
        {
            Server server = new Server();
            server.Start();
            eventHandle.Set();
            Console.WriteLine("Press enter to stop server....");
            Console.ReadLine(); // Keep the server running
        }
    }
}

