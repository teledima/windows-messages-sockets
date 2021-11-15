﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HelperSockets;
using RabbitMQ.Client;

namespace WindowsMessagesSockets
{
    public class ClientMessages
    {
        private readonly IDisplayMessage _displayMessage;
        public ClientMessages(IDisplayMessage displayMessage)
        {
            _displayMessage = displayMessage;
        }     
        public void SendData(IEnumerable<SourceGames> sourceGames)
        {
            try
            {
                var factory = new ConnectionFactory();

                factory.UserName = "guest";
                factory.Password = "guest";
                factory.VirtualHost = "/";
                factory.HostName = HelperSockets.Properties.Settings.Default["host"].ToString();

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    // Get AES key
                    RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                    RSA.FromXmlString(Encoding.UTF8.GetString(MQServerHelper.RecieveData(channel, "RSA")));

                    // Send DES key
                    var desServive = new DesService();
                    byte[] DESEncrypt = RSA.Encrypt(desServive.Serialize(), false);
                    MQServerHelper.SendData(channel, "DES", DESEncrypt);

                    // Send rows
                    foreach (var sourceGame in sourceGames)
                    {
                        var encryptedData = SourceGamesHelper.Encrypt(new List<SourceGames>() { sourceGame }, desServive);
                        MQServerHelper.SendData(channel, "SourceGames", encryptedData);
                    }
                }
                _displayMessage.Display("Отправлено.\n");
            }
            catch (Exception ex)
            {
                _displayMessage.Display(string.Format("Client sendMQ error: {0}", ex.Message));
            }
        }
    }


}
