using System;
using System.IO.MemoryMappedFiles;
using System.Text;

class Reader
{
    static void Main(string[] args)
    {
        using (EventWaitHandle eventHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\MyIPCEvent"))
        {
            Console.WriteLine("Reader: Waiting for signal...");

            // Wait for the signal (event to be set)
            eventHandle.WaitOne();
            eventHandle.Reset();
            // Open the named memory-mapped file
            using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("sharedMemory"))
            {
                
                // Create a view accessor to read from the memory
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    // Read the length of the message (first 4 bytes)
                    int messageLength = accessor.ReadInt32(0);

                    // Read the message itself
                    byte[] messageBytes = new byte[messageLength];
                    accessor.ReadArray(4, messageBytes, 0, messageLength);

                    // Convert the byte array to a string
                    string message = Encoding.UTF8.GetString(messageBytes);

                    Console.WriteLine("Data read from shared memory: " + message);
                }
            }
            eventHandle.Set();
        }

        Console.WriteLine("Press any key...");
        Console.ReadKey();
    }
}
