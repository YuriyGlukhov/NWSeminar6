using NWSeminar5.Abstraction;
using NWSeminar5;
using Moq;
using System.Net;
using Microsoft.VisualBasic;
using System.Reflection;

namespace TestProject2
{
    public class ClientTests
    {
        private Mock<IMessageSource> _mockMessageSource;
        private Client _client;
        private IPEndPoint _endpoint;
        private CancellationTokenSource _cts;

        [SetUp]
        public void SetUp()
        {
            _mockMessageSource = new Mock<IMessageSource>();

            _endpoint = new IPEndPoint(IPAddress.Loopback, 12345);

            _client = new Client(_mockMessageSource.Object, "Вася", _endpoint);
            _cts = new CancellationTokenSource();
        }
        [TearDown]
        public void TearDown()
        {
            _cts.Cancel(); 
            _cts.Dispose(); 
        }

        [Test]
        public async Task TestClientStartAndRegister()
        {            
           
            var registerMessage = new MessageUDP
            {
                Command = Command.Register,
                FromName = _client._name,
            };

            _mockMessageSource.Setup(m => m.SendMessage(It.IsAny<MessageUDP>(), It.IsAny<IPEndPoint>()));

            var clientTask = Task.Run(() => _client.StartClientAsync(), _cts.Token); 

            await Task.Delay(500); 

            _mockMessageSource.Verify(m => m.SendMessage(It.Is<MessageUDP>(msg => msg.Command == Command.Register
                               && msg.FromName == "Вася"), It.IsAny<IPEndPoint>()), Times.Once);

            _cts.Cancel(); 
            await Task.Delay(100); 
            
        }

        [Test]
        public void TestClientSender()
        {

            var sendMessage = new MessageUDP
            {
                Command = Command.Message,
                FromName = _client._name,
                Text = "Тестовый текст",
                ToName = "Петя"
            };


            _mockMessageSource.Setup(m => m.SendMessage(It.IsAny<MessageUDP>(), It.IsAny<IPEndPoint>()));

            _mockMessageSource.Object.SendMessage(sendMessage, _client._endpoint);
             
            _mockMessageSource.Verify(m => m.SendMessage(It.Is<MessageUDP>(msg => msg.Command == Command.Message
                               && msg.FromName == "Вася"
                               && msg.ToName == "Петя"
                               && msg.Text == "Тестовый текст"),
                               It.IsAny<IPEndPoint>()), Times.Once);

        }

        [Test]
        public async Task TestClientListener()
        {
            
            var getMessage = new MessageUDP
            {
                Command = Command.Message,
                FromName = "Петя",
                Text = "Ответ на текстовый текст",
                ToName = _client._name
            };


            _mockMessageSource.Setup(x => x.ReceiveMessage(ref _endpoint)).Returns(getMessage);

            var clientTask = Task.Run(() => _client.StartClientAsync(), _cts.Token);

            await Task.Delay(500);

            _mockMessageSource.Verify(x => x.ReceiveMessage(ref It.Ref<IPEndPoint>.IsAny), Times.AtLeastOnce);

            _cts.Cancel();
            await Task.Delay(500);
        }

        [Test]
        public void TestClientExit()
        {

            var sendMessage = new MessageUDP
            {
                Command = Command.Exit,
                FromName = _client._name,
               
            };


            _mockMessageSource.Setup(m => m.SendMessage(It.IsAny<MessageUDP>(), It.IsAny<IPEndPoint>()));

            _mockMessageSource.Object.SendMessage(sendMessage, _client._endpoint);

            _mockMessageSource.Verify(m => m.SendMessage(It.Is<MessageUDP>(msg => msg.Command == Command.Exit
                               && msg.FromName == "Вася"),
                               It.IsAny<IPEndPoint>()), Times.Once);

        }
    }
}