using System.Net;

namespace TestProject1
{
    public class MockMessageSource : IMessageSource
    {
        private Queue<MessageUDP> messages = new();
        private Server server;
        private IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        public MockMessageSource()
        {
            messages.Enqueue(new Message
            {
                Command = Command.Register,
                FromName =
            "Вася"
            });
            messages.Enqueue(new Message
            {
                Command = Command.Register,
                FromName = "Юля"
            });
            messages.Enqueue(new Message
            {
                Command = Command.Message,
                FromName = "Юля",
                ToName = "Вася",
                Text = "От Юли"
            });
            messages.Enqueue(new Message
            {
                Command = Command.Message,
                FromName =
            "Вася",
                ToName = "Юля",
                Text = "От Васи"
            });
        }
        public void AddServer(Server srv)
        {
            server = srv;
        }
        public Message Receive(ref IPEndPoint ep)
        {
            ep = endPoint;
            if (messages.Count == 0)
            {
                server.Stop();
                return null;
            }
            var msg = messages.Dequeue();
            return msg;
        }
        public void Send(Message message, IPEndPoint ep)
        {
            //throw new NotImplementedException();
        }
    }

    public interface IMessageSource
    {
    }

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            using (var ctx = new Lection15Program6.Model.TestContext())
            {
                ctx.Messages.RemoveRange(ctx.Messages);
                ctx.Users.RemoveRange(ctx.Users);
                ctx.SaveChanges();
            }
        }
        [TearDown]
        public void TeatDown()
        {
            using (var ctx = new Lection15Program6.Model.TestContext())
            {
                ctx.Messages.RemoveRange(ctx.Messages);
                ctx.Users.RemoveRange(ctx.Users);
                ctx.SaveChanges();
            }
        }
        [Test]
        public void Test1()
        {
            var mock = new MockMessageSource();
            var srv = new Server(mock);
            mock.AddServer(srv);
            srv.Work();
            using (var ctx = new Lection15Program6.Model.TestContext())
            {
                Assert.IsTrue(ctx.Users.Count() == 2, "Пользователи не созданы");
                var user1 = ctx.Users.FirstOrDefault(x => x.Name == "Вася");
                var user2 = ctx.Users.FirstOrDefault(x => x.Name == "Юля");
                Assert.IsNotNull(user1, "Пользователь не созданы");
                Assert.IsNotNull(user2, "Пользователь не созданы");
                Assert.IsTrue(user1.FromMessages.Count == 1);
                Assert.IsTrue(user2.FromMessages.Count == 1);
                Assert.IsTrue(user1.ToMessages.Count == 1);
                Assert.IsTrue(user2.ToMessages.Count == 1);
                var msg1 = ctx.Messages.FirstOrDefault(x => x.FromUser == user1 &&
                x.ToUser == user2);
                var msg2 = ctx.Messages.FirstOrDefault(x => x.FromUser == user2 &&
                x.ToUser == user1);
                Assert.AreEqual("От Юли", msg2.Text);
                Assert.AreEqual("От Васи", msg1.Text);


            }
        }
    }
}
