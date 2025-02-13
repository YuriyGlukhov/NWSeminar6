using NWSeminar5.Abstraction;
using System;
using System.Net;
using System.Net.Sockets;

namespace NWSeminar5
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Укажите режим работы: server или client [имя клиента] [порт клиента]");
                return;
            }

            string mode = args[0].ToLower();

            if (mode == "server")
            {
                int serverPort = 12345;
                IMessageSource serverSource = new MessageSource(serverPort);
                Server server = new Server(serverSource);

                Console.WriteLine("Запуск сервера...");
                server.StartServer();
            }
            else if (mode == "client")
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Для клиента укажите имя и порт! Пример: client Client1 12346");
                    return;
                }

                string clientName = args[1];
                int clientPort = int.Parse(args[2]);

                IMessageSource clientSource = new MessageSource(clientPort);
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 12345);

                Client client = new Client(clientSource, clientName, serverEndPoint);

                Console.WriteLine($"Запуск клиента {clientName} на порту {clientPort}...");
                await client.StartClientAsync();
            }
            else
            {
                Console.WriteLine("Неизвестный режим. Используйте: server или client [имя клиента] [порт клиента]");
            }
        }
    }
}
