using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace HelperSockets
{
    public static class MQServerHelper
    {
        public static void SendData(IModel channel, string nameQueue, byte[] data)
        {
            channel.BasicPublish("", nameQueue, null, data);
        }

        public static byte[] RecieveData(IModel channel, string nameQueue)
        {
            var consumer = new EventingBasicConsumer(channel);

            byte[] data = null;

            consumer.Received += (model, ea) =>
            {
                data = ea.Body.ToArray();
            };

            string tag = channel.BasicConsume(nameQueue, true, consumer);
            while (data == null) ;
            channel.BasicCancelNoWait(tag);
            return data;
        }
    }
}
