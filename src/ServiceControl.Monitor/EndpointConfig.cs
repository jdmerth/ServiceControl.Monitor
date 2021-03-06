namespace EaiGuy.ServiceControl.Monitor
{
    using NServiceBus;
    using NServiceBus.Log4Net;
    using NServiceBus.Logging;
    using NServiceBus.Persistence;

    /*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://particular.net/articles/the-nservicebus-host
	*/
    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
            log4net.Config.XmlConfigurator.Configure();
            LogManager.Use<Log4NetFactory>();

            // configure persistence
            configuration.UsePersistence<RavenDBPersistence>();

            // use json serializer to be compatible with MessageFailed messages coming from ServiceControl; this means we can't send any messages to any other ESB services, since they use XML serialization
            configuration.UseSerialization<JsonSerializer>();

            var conventionsBuilder = configuration.Conventions();
            conventionsBuilder.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands") && t.Namespace.StartsWith("EaiGuy"));
            conventionsBuilder.DefiningEventsAs(t => t.Namespace != null && (t.Namespace.EndsWith("Events") && t.Namespace.StartsWith("EaiGuy")
               || t.Namespace == "ServiceControl.Contracts"));
            conventionsBuilder.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages") && t.Namespace.StartsWith("EaiGuy"));
        }
    }
}
