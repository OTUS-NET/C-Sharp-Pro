using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using CalculatorService;

namespace WCFServerHost
{
    class Server
    {
        static void Main(string[] args)
        {
            // Создание URI для сервиса
            Uri baseAddress = new Uri("http://localhost:8080/CalculatorService");

            // Создание экземпляра хоста
            using (ServiceHost host = new ServiceHost(typeof(Calculator)))
            {
                // Настройка привязки (binding)
                //host.AddServiceEndpoint(typeof(ICalculator), new BasicHttpBinding(), "");

                //// Включаем поддержку метаданных (MEX)
                //ServiceMetadataBehavior smb = new ServiceMetadataBehavior
                //{
                //    HttpGetEnabled = true
                //};
                //host.Description.Behaviors.Add(smb);

                //// Добавляем MEX конечную точку для обмена метаданными
                //host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

                // Открытие хоста
                host.Open();
                Console.WriteLine("Сервис запущен. Нажмите Enter для завершения работы...");
                Console.ReadLine();
            }
        }
    }
}
