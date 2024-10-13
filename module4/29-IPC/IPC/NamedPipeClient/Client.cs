using System.IO.Pipes;

class NamedPipeClient
{
    static void Main(string[] args)
    {
        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "MyPipe", PipeDirection.InOut))
        {
            Console.WriteLine("Connecting to server...");
            pipeClient.Connect();
            Console.WriteLine("Connected to server.");

            using (StreamReader reader = new StreamReader(pipeClient))
            using (StreamWriter writer = new StreamWriter(pipeClient) { AutoFlush = true })
            {
                while (true)
                {
                    Console.Write("Enter message: ");
                    string message = Console.ReadLine();
                    writer.WriteLine(message);

                    if (message == "exit")
                    {
                        break;
                    }

                    string response = reader.ReadLine();
                    Console.WriteLine("Server response: " + response);
                }
            }
        }
    }
}
