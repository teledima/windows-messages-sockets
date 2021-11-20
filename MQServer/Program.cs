using RabbitMQ.Client;
using System.Security.Cryptography;
using System.Text;
using HelperSockets;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using System;

namespace MQServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var RSA = new RSACryptoServiceProvider(4096);

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
                        var desService = AesService.FromBytes(bytesDES);
                        displayMessage.Display("DES ключ получен");

                        // Consumer for rows
                        var consumer = new EventingBasicConsumer(channel);

                        byte[] data = null;

                        consumer.Received += (model, ea) =>
                        {
                            data = desService.Decrypt(ea.Body.ToArray());
                            displayMessage.Display(string.Format("Запись получена, размер: {0} байт", data.Length));

                            var rows = SourceGamesHelper.Parse(data);
                            try
                            {
                                // Export data
                                SourceGamesHelper.ExportToPostgres(rows);
                            }
                            catch (Exception e)
                            {
                                displayMessage.Display(e.Message);
                            }
                        };
                        displayMessage.Display("Все данные получены");
                        string tag = channel.BasicConsume("SourceGames", true, consumer);
                    }
                }
            }
        }
    }
}
