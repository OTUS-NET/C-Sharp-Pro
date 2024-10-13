using System;
using System.Diagnostics;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Thread.Sleep(3000);

        string logName = "MyCustomLog";
        string sourceName = "EventLogWriterApp";

        // Проверяем, существует ли журнал
        if (!EventLog.Exists(logName))
        {
            Console.WriteLine($"Журнал {logName} не существует.");
            return;
        }

        // Асинхронное чтение сообщений из EventLog
        await ReadFromEventLogAsync(logName, sourceName);

        Console.ReadLine();
    }

    static async Task ReadFromEventLogAsync(string log, string source)
    {
        await Task.Run(() =>
        {
            using (EventLog eventLog = new EventLog(log))
            {
                foreach (EventLogEntry entry in eventLog.Entries)
                {
                    if (entry.Source == source)// && (entry.EntryType == EventLogEntryType.Error || entry.EntryType == EventLogEntryType.Warning))
                    {
                        Console.WriteLine($"[{entry.TimeGenerated}] {entry.EntryType}: {entry.Message}");
                    }
                }
            }
        });
    }
}
