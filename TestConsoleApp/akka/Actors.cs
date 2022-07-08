using Akka.Actor;
using PoiskIT.Andromeda.interfases.akka;
using PoiskIT.Andromeda.Akka.Test.Items;

namespace PoiskIT.Andromeda.Akka.Test.Actors
{
    /// <summary>
    /// The class simple actor. 
    /// </summary>
    public class GreetingActor : ReceiveActor
    {
        public GreetingActor()
        {
            // Sey actor get reaction for this.
            Receive<Greet>(greet => Console.WriteLine($"Hello {greet.Who}!"));
        }
    }

    public class NotificationActor : UntypedActor
    {
        private readonly IEmailNotification emailNotification;

        public NotificationActor(IEmailNotification emailNotification)
        {
            this.emailNotification = emailNotification;
        }

        protected override void OnReceive(object message)
        {
            if (message == null)
                return;

            Console.WriteLine($"Message received {message}");
            emailNotification.Send(message.ToString());
        }

        protected override void PreStart() => Console.WriteLine("Actor started");

        protected override void PostStop() => Console.WriteLine("Actor stopped");
    }
}
