using NWSeminar5.Abstraction;
using NWSeminar5;
using System.Net;
using NWSeminar5.Models;

namespace TestProject2
{
    public class MockMessageSource : IMessageSource
    {
        private Queue<MessageUDP> messages = new Queue<MessageUDP>();
        private Server server;
        private IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        public MockMessageSource()
        {
            messages.Enqueue(new MessageUDP
            {
                Command = Command.Register,
                FromName = "Вася"
            });
            messages.Enqueue(new MessageUDP
            {
                Command = Command.Register,
                FromName = "Юля"
            });
            messages.Enqueue(new MessageUDP
            {
                Command = Command.Message,
                FromName = "Юля",
                ToName = "Вася",
                Text = "От Юли"
            });
            messages.Enqueue(new MessageUDP
            {
                Command = Command.Message,
                FromName =
            "Вася",
                ToName = "Юля",
                Text = "От Васи"
            });
        }

        public MessageUDP ReceiveMessage(ref IPEndPoint endPoint)
        {
            return messages.Last();
            
        }

        public void SendMessage(MessageUDP message, IPEndPoint endPoint)
        {
            messages.Enqueue(message);
        }

     
    }
    public class Tests
    {
        IPEndPoint _endPoint;
        IMessageSource _messageSource;


        [SetUp]
        public void Setup()
        {
            _endPoint = new IPEndPoint(IPAddress.Any, 0);
          
        }
       
        [Test]
        public void TestReceiveMessage()
        {
            _messageSource = new MockMessageSource();
            var result = _messageSource.ReceiveMessage(ref _endPoint);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Text);
            Assert.IsNotNull(result.FromName);
            Assert.That("Вася", Is.EqualTo(result.FromName));
            Assert.That(Command.Register, Is.EqualTo(result.Command));

        }
        [Test]
        public void TestSendMessage()
        {
            var source = new MockMessageSource();

            var sendMessage1 = new MessageUDP
            {
                Command = Command.Message,
                FromName = "Вася",
                ToName = "Юля",
                Text = "Привет, Юля"
            };

            source.SendMessage(sendMessage1, _endPoint);

            var receivedMessage1 = source.ReceiveMessage(ref _endPoint);

            Assert.IsNotNull(receivedMessage1);
            Assert.IsNotNull(receivedMessage1.Text);
            Assert.IsNotNull(receivedMessage1.ToName);
            Assert.IsNotNull(receivedMessage1.FromName);
            Assert.That(receivedMessage1.Text, Is.EqualTo("Привет, Юля"));
            Assert.That(receivedMessage1.FromName, Is.EqualTo("Вася"));
            Assert.That(receivedMessage1.ToName, Is.EqualTo("Юля"));
            Assert.That(receivedMessage1.Command, Is.EqualTo(Command.Message));


            var sendMessage2 = new MessageUDP
            {
                Command = Command.Message,
                FromName = "Юля",
                ToName = "Вася",
                Text = "И тебе привет, Вася"
            };

            source.SendMessage(sendMessage2, _endPoint);

            var receivedMessage2 = source.ReceiveMessage(ref _endPoint);

            Assert.IsNotNull(receivedMessage2);
            Assert.IsNotNull(receivedMessage2.Text);
            Assert.IsNotNull(receivedMessage2.ToName);
            Assert.IsNotNull(receivedMessage2.FromName);
            Assert.That(receivedMessage2.Text, Is.EqualTo("И тебе привет, Вася"));
            Assert.That(receivedMessage2.FromName, Is.EqualTo("Юля"));
            Assert.That(receivedMessage2.ToName, Is.EqualTo("Вася"));
            Assert.That(receivedMessage2.Command, Is.EqualTo(Command.Message));
        }   
    }
}