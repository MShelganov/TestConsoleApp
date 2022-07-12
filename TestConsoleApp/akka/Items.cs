using PoiskIT.Andromeda.interfases.akka;

namespace PoiskIT.Andromeda.Akka.Test.Items
{
    /// <summary>
    /// The type of messages for actor's reaction.
    /// </summary>
    public class Greet
    {
        public string Who { get; init; }
        public Greet(string who)
        {
            Who = who;
        }
    }

    public class EmailNotification : IEmailNotification
    {
        public void Send(string message)
        {
            Console.WriteLine($"Email sent with message: {message} &");
        }
    }
}
