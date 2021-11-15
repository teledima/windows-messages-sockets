using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using HelperSockets;
using RabbitMQ.Client.Events;

namespace MQServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var RSA = new RSACryptoServiceProvider(2048);

            var serverIp = HelperSockets.Properties.Settings.Default["host"].ToString();
            Console.WriteLine("Ip адрес сервера: " + serverIp);
            Console.WriteLine("Сервер запущен.");

            var factory = new ConnectionFactory()
            {
                HostName = serverIp,
                Password = "guest",
                UserName = "guest"
            };
            using (var connection = factory.CreateConnection())
            {
                while (true)
                {
                    using (var channel = connection.CreateModel())
                    {
                        MQServerHelper.SendData(channel, "RSA", Encoding.UTF8.GetBytes(RSA.ToXmlString(false)));

                        // Get des service
                        var bytesDES = RSA.Decrypt(MQServerHelper.RecieveData(channel, "DES"), false);
                        var desService = DesService.FromBytes(bytesDES);

                        // Consumer for rows
                        var consumer = new EventingBasicConsumer(channel);

                        byte[] data = null;

                        consumer.Received += (model, ea) =>
                        {
                            data = desService.Decrypt(ea.Body.ToArray());
                            var rows = SourceGamesHelper.Parse(data);
                            SourceGamesHelper.ExportToPostgres(rows);
                            Console.WriteLine("Запись добавлена");
                        };

                        string tag = channel.BasicConsume("SourceGames", true, consumer);
                    }
                }
            }
        }
    }
}
