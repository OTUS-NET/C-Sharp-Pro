using System;
using System.ServiceModel;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Создание клиента для взаимодействия с сервисом
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress("http://localhost:8080/CalculatorService");
            var factory = new ChannelFactory<ICalculator>(binding, endpoint);

            ICalculator calculator = factory.CreateChannel();

            try
            {
                // Ввод чисел
                Console.Write("Введите первое число: ");
                int num1 = int.Parse(Console.ReadLine());

                Console.Write("Введите второе число: ");
                int num2 = int.Parse(Console.ReadLine());

                // Вызов сервиса для сложения чисел
                int result = calculator.Add(num1, num2);
                Console.WriteLine($"Результат сложения: {result}");
            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка: введите допустимые числа.");
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine($"Ошибка связи с сервисом: {ex.Message}");
            }
            finally
            {
                if (calculator is ICommunicationObject commObj)
                {
                    commObj.Close();
                }
            }
        }
    }
}
