using System;
using System.IO.MemoryMappedFiles;
using System.Text;

class Writer
{
    static void Main(string[] args)
    {
        using (EventWaitHandle eventHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\MyIPCEvent"))
        {
            // Create a named memory-mapped file
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("sharedMemory", 1024))
            {
                // Create a view accessor to write to the memory
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    // Message to write to shared memory
                    string message = "Hello from the shared memory!";
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                    // Write the length of the message (first 4 bytes)
                    accessor.Write(0, messageBytes.Length);
                     
                    // Write the message itself
                    accessor.WriteArray(4, messageBytes, 0, messageBytes.Length);

                    Console.WriteLine("Data written to shared memory.");
                }

                Console.WriteLine("Writer: Press any key to send the signal...");
                Console.ReadKey();  // Wait for user input to simulate signal sending

                // Set the event (send signal)
                eventHandle.Set();
                Console.WriteLine("Sender: Signal sent.");
                Thread.Sleep(1000);
                //wait when reader red all data
                eventHandle.WaitOne();
            }
        }

        Console.WriteLine("Press any key...");
        Console.ReadKey();  
    }
}