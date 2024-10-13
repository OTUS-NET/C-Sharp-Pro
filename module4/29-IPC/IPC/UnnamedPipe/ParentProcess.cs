using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
using System.Threading.Tasks;

class ParentProcess
{
    static async Task Main(string[] args)
    {
        // Создаем канал для обмена данными (родительский процесс пишет, дочерний читает)
        using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
        {
            // Запускаем дочерний процесс и передаем ему дескриптор канала
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --project ..\\..\\..\\..\\UnnamedPipeChildProcess " + pipeServer.GetClientHandleAsString(),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (var childProcess = Process.Start(processStartInfo))
            {
                // Закрываем клиентский хэндл в родительском процессе
                pipeServer.DisposeLocalCopyOfClientHandle();

                Console.WriteLine("Ожидание данных от дочернего процесса...");

                // Чтение данных из дочернего процесса
                using (StreamReader reader = new StreamReader(pipeServer))
                {
                    string messageFromChild = await reader.ReadLineAsync();
                    Console.WriteLine($"Получено сообщение от дочернего процесса: {messageFromChild}");

                    // Пример обработки данных: добавим текст
                    string processedMessage = $"[Обработано родителем]: {messageFromChild.ToUpper()}";
                    Console.WriteLine($"Обработанное сообщение: {processedMessage}");
                }

                // Ожидание завершения дочернего процесса
                await childProcess.WaitForExitAsync();
            }
        }

        Console.ReadLine();
    }
}
