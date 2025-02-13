using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NWSeminar5.Abstraction
{
    public interface IMessageSource
    {
        public void SendMessage(MessageUDP message, IPEndPoint endPoint);


        public MessageUDP ReceiveMessage(ref IPEndPoint endPoint);
        
    }
}
