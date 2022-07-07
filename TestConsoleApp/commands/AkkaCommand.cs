using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Extensions.DependencyInjection;
using TestConsoleApp.interfases;
using TestConsoleApp.interfases.akka;
using TestConsoleApp.Akka.Test.Actors;
using TestConsoleApp.Akka.Test.Items;
using Microsoft.Extensions.DependencyInjection;

namespace TestConsoleApp.commands
{
    internal class AkkaCommand : BaseCommand, ICommand
    {
        public override string Name => "akka";

        public override string Description => "Akka test.";

        public override void Execute(string[]? subcommand = null)
        {
            // Create an actors' system (container)
            using (var system = ActorSystem.Create("NameOfSystem"))
            {
                var serviceProvider = GetServiceProvider();
                system.UseServiceProvider(serviceProvider);
                Notification(system);
            }
        }

        public void HelloAkka(IActorRefFactory system)
        {
            // Create own actor and get reference at him.
            // It "ActorRef" is client or proxy to him
            // do not a link at current instance of the actor
            var greeter = system.ActorOf<GreetingActor>("greeter");

            // Send message
            greeter.Tell(new Greet("World"));
        }

        public void Notification(ActorSystem system)
        {
            // var notification = system.ActorOf(Props.Create<NotificationActor>(), "notification-actor");
            var notification = system.ActorOf(system.DI().Props<NotificationActor>(), "notification-actor");

            notification.Tell("Test notification");
            notification.Tell("Test notification 2");

            //system.Stop(notification); // Fails to work
        }

        public IServiceProvider GetServiceProvider()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IEmailNotification, EmailNotification>();
            collection.AddSingleton<NotificationActor>();

            var provider = collection.BuildServiceProvider();

            return provider;
        }
    }
}
