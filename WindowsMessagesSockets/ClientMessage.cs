using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HelperSockets;
using RabbitMQ.Client;

namespace WindowsMessagesSockets
{
    public class ClientMessage: AbstractClient
    {
        public ClientMessage(IDisplayMessage displayMessage) : base(displayMessage) { }
        public override void SendData(IEnumerable<SourceGames> sourceGames)
        {
            try
            {
                var factory = new ConnectionFactory();

                factory.UserName = "admin";
                factory.Password = "admin";
                factory.Port = 5672;
                factory.VirtualHost = "/";
                factory.HostName = HelperSockets.Properties.Settings.Default["host"].ToString();
                factory.RequestedHeartbeat = new TimeSpan(0, 0, 60);

                this.displayMessage.Display("Try connect...\n");
                using (var connection = factory.CreateConnection())
                {
                    this.displayMessage.Display(string.Format("Connected to {0}\n",connection.Endpoint.HostName));

                    using (var channel = connection.CreateModel())
                    {
                        // Get AES key
                        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                        RSA.FromXmlString(Encoding.UTF8.GetString(MQServerHelper.RecieveData(channel, "RSA")));
                        this.displayMessage.Display("Получен RSA ключ из сервера\n");

                        // Send DES key
                        var aesServive = new AesService();
                        byte[] AESEncrypt = RSA.Encrypt(aesServive.Serialize(), false);
                        MQServerHelper.SendData(channel, "DES", AESEncrypt);
                        this.displayMessage.Display("Отправлен DES ключ серверу\n");

                        // Send rows
                        foreach (var sourceGame in sourceGames)
                        {
                            var encryptedData = SourceGamesHelper.Encrypt(new List<SourceGames>() { sourceGame }, aesServive);
                            MQServerHelper.SendData(channel, "SourceGames", encryptedData);
                            this.displayMessage.Display(string.Format("Запись отправлена, размер: {0} байт\n", encryptedData.Length));

                        }

                    }
                    this.displayMessage.Display("Все данные отправлены.\n");
                }
            }
            catch (Exception ex)
            {
                this.displayMessage.Display(string.Format("Client sendMQ error: {0}\n", ex.Message));
            }
        }
    }


}
