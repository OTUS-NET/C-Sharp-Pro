using System;
using System.Diagnostics;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string logName = "MyCustomLog";
        string sourceName = "EventLogWriterApp";

        // Проверяем, существует ли журнал
        if (!EventLog.SourceExists(sourceName))
        {
            // Создаем журнал, если его нет
            EventLog.CreateEventSource(sourceName, logName);
            Console.WriteLine($"Создан журнал {logName} с источником {sourceName}.");
        }

        // Асинхронная запись в EventLog
        await WriteToEventLogAsync(sourceName, logName, "Это информационное сообщение.", EventLogEntryType.Information);
        await WriteToEventLogAsync(sourceName, logName, "Это предупреждение.", EventLogEntryType.Warning);
        await WriteToEventLogAsync(sourceName, logName, "Это сообщение об ошибке.", EventLogEntryType.Error);

        Console.WriteLine("Сообщения асинхронно записаны в EventLog.");

        Console.ReadLine();
    }

    static async Task WriteToEventLogAsync(string source, string log, string message, EventLogEntryType type)
    {
        await Task.Run(() =>
        {
            using (EventLog eventLog = new EventLog(log))
            {
                eventLog.Source = source;
                eventLog.WriteEntry(message, type);
            }
        });
    }
}
