﻿using RabbitMQ.Client;
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
            var displayMessage = new DisplayConsole();
            displayMessage.Display("Ip адрес сервера: " + serverIp);
            displayMessage.Display("Сервер запущен.");

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
                        displayMessage.Display("RSA ключ отправлен");

                        // Get des service
                        var bytesDES = RSA.Decrypt(MQServerHelper.RecieveData(channel, "DES"), false);
                        var desService = DesService.FromBytes(bytesDES);
                        displayMessage.Display("DES ключ получен");

                        // Consumer for rows
                        var consumer = new EventingBasicConsumer(channel);

                        byte[] data = null;

                        consumer.Received += (model, ea) =>
                        {
                            data = desService.Decrypt(ea.Body.ToArray());
                            displayMessage.Display(string.Format("Запись получена, размер: {0} байт", data.Length));

                            var rows = SourceGamesHelper.Parse(data);
                            SourceGamesHelper.ExportToPostgres(rows);
                        };
                        displayMessage.Display("Все данные получены");
                        string tag = channel.BasicConsume("SourceGames", true, consumer);
                    }
                }
            }
        }
    }
}
