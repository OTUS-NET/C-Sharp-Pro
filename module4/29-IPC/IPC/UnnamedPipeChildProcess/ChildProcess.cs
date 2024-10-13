using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

class ChildProcess
{
    static async Task Main(string[] args)
    {
        // Получаем дескриптор канала из аргументов командной строки
        string pipeHandle = args[0];

        // Подключаемся к каналу, используя переданный дескриптор
        using (var pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, pipeHandle))
        {
            using (StreamWriter writer = new StreamWriter(pipeClient))
            {
                writer.AutoFlush = true;

                // Отправляем сообщение родительскому процессу
                string message = "Привет из дочернего процесса!";
                await writer.WriteLineAsync(message);

                Console.WriteLine("Сообщение отправлено родительскому процессу.");

                //Console.ReadLine();

                //Thread.Sleep(1000);
            }
        }
    }
}
