using NWSeminar5.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NWSeminar5
{
    public class MessageSource : IMessageSource
    {
        private readonly UdpClient _udpClient;

        public MessageSource(int port)
        {
            _udpClient = new UdpClient(port);

        }
     
        public MessageUDP ReceiveMessage(ref IPEndPoint endPoint)
        {
            byte[] data = _udpClient.Receive(ref endPoint);
            string json = Encoding.UTF8.GetString(data);
            return MessageUDP.FromJson(json);
        }

      
        public void SendMessage(MessageUDP message, IPEndPoint endPoint)
        {
            string json = message.ToJson();
            byte[] data = Encoding.UTF8.GetBytes(json);
            _udpClient.Send(data, data.Length, endPoint);
        }
    }
}
