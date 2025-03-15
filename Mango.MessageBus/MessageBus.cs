using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {

        //private readonly string connectionString;



        private readonly string connectionString = "Endpoint=sb://yossimicroweb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=+Ke5UrYzJ+LKMnqjxa0xK/wS3oR2gHGvH+ASbK4EtE4=";

        public async Task PublicMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(connectionString);
            
            ServiceBusSender sender = client.CreateSender(topic_queue_Name);
            var jsonMessage =JsonConvert.SerializeObject(message);

            ServiceBusMessage finalmessage = new ServiceBusMessage(Encoding
                .UTF8.GetBytes(jsonMessage))
            {
                CorrelationId=Guid.NewGuid().ToString(),
            };
            await sender.SendMessageAsync(finalmessage);
            await client.DisposeAsync();
        }
    }
}
