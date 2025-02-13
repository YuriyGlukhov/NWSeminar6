using NWSeminar5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Abstractions;
using System.Xml.Linq;
using NWSeminar5.Abstraction;
using Microsoft.IdentityModel.Tokens;
using System.Threading;

namespace NWSeminar5
{
    public class Client
    {
        public readonly string _name;
        private readonly IMessageSource _messageSource;
        public IPEndPoint _endpoint;
        public static CancellationTokenSource cts = Server.cts;

        public Client(IMessageSource messageSource, string name, IPEndPoint endPoint)
        {
            _messageSource = messageSource;
            _endpoint = endPoint;  
            _name = name;   
        }
        public async Task StartClientAsync()
        {
            Task listenerTask = Task.Run(() => ClientListener());
            Task senderTask = Task.Run(() => ClientSender());
            Registered();

            await Task.WhenAny(listenerTask, senderTask);
        }
        private void Registered()
        {
            var messJson = new MessageUDP()
            {
                Command = Command.Register,
                FromName = _name,
            };

            _messageSource.SendMessage(messJson, _endpoint);
        }

        public void Exit()
       {
            var messJson = new MessageUDP()
            {
                Command = Command.Exit,
                FromName = _name,
            };
            cts.Cancel();
            _messageSource.SendMessage(messJson, _endpoint);
            
        }
        public void ClientListener()
        {
            while (!cts.IsCancellationRequested)
            {
               
                MessageUDP message = _messageSource.ReceiveMessage(ref _endpoint);
                if (message != null)
                {
                    Console.WriteLine($"\n{message}");
                }
                
            }

        }

        public void ClientSender()
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    Thread.Sleep(1000);
                    Console.Write("Введите сообщение: ");
                    string? input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                    {
                        Console.WriteLine("Вы не ввели сообщение");
                        continue;
                    }
                    if (input == "exit".ToLower())
                    {
                        Exit();
                        
                        break;
                    }

                    Console.Write("Введите имя получателя: ");
                    string? toName = Console.ReadLine();

                    if (string.IsNullOrEmpty(toName))
                    {
                        Console.WriteLine("Вы не ввели имя");
                        continue;
                    }

                    var messJson = new MessageUDP()
                    {
                        STime = DateTime.Now,
                        Text = input,
                        FromName = _name,
                        ToName = toName,
                        Command = Command.Message
                        
                    };

                    _messageSource.SendMessage(messJson, _endpoint);

                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Произошла ошибка {ex}");
                }
                    
            }
        }
    }
}
