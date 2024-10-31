using System.IO.Pipes;

class NamedPipeServer
{
    static void Main(string[] args)
    {
        int maxClients = 2;  // Set the maximum number of clients
        Thread[] clientThreads = new Thread[maxClients];

        for (int i = 0; i < maxClients; i++)
        {
            clientThreads[i] = new Thread(ServerWorker);
            clientThreads[i].Start();
        }

        foreach (Thread thread in clientThreads)
        {
            thread.Join();
        }
    }

    // Function to handle client communication
    static void ServerWorker()
    {
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("MyPipe", PipeDirection.InOut, 2))
        {
            Console.WriteLine("Waiting for client connection...");
            pipeServer.WaitForConnection();
            Console.WriteLine("Client connected.");

            using (StreamReader reader = new StreamReader(pipeServer))
            using (StreamWriter writer = new StreamWriter(pipeServer) { AutoFlush = true })
            {
                while (true)
                {
                    string message = reader.ReadLine();
                    if (message == "exit")
                    {
                        Console.WriteLine("Client disconnected.");
                        break;
                    }
                    Console.WriteLine("Received message from client: " + message);
                    writer.WriteLine("Echo: " + message); // Echo back the message
                }
            }
        }
    }
}
