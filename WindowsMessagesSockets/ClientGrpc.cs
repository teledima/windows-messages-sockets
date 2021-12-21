using Grpc.Core;
using Grpc.Net.Client;
using HelperSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WindowsMessagesSockets
{
    public class ClientGrpc: AbstractClient
    {
        public ClientGrpc(IDisplayMessage displayMessage) : base(displayMessage) { }
        public override void SendData(IEnumerable<SourceGames> sourceGames)
        {
            // Read SSL certificate
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            handler.ClientCertificates.Add(new X509Certificate2(Properties.Settings.Default["certificate_path"].ToString()));
            using var httpClient = new HttpClient(handler);

            using var channel = GrpcChannel.ForAddress(string.Format("http://{0}:{1}", 
                HelperSockets.Properties.Settings.Default["host"].ToString(), int.Parse(HelperSockets.Properties.Settings.Default["port"].ToString())
            ));
            var client = new Exporter.ExporterClient(channel);

            displayMessage.Display("Start gRPC call\n");
            var callOptions = new CallOptions(deadline: DateTime.UtcNow.AddMinutes(2));
            try
            {
                foreach (SourceGames sourceGame in sourceGames)
                {
                    var row = new Row()
                    {
                        GameName = sourceGame.GamesName,
                        AchievementName = sourceGame.AchievementsName,
                        CategoryName = sourceGame.CategoriesName,
                        DownloadableContentName = sourceGame.DownloadableContentsName
                    };
                    var reply = client.Export(row, callOptions);
                    displayMessage.Display(string.Format("{0} - Status export: {1}\n", sourceGame.Id, reply.Ok));
                }
            }
            catch (RpcException ex)
            {
                displayMessage.Display(string.Format("{0}, status code: {1}\n", ex.Status.Detail, ((uint)ex.StatusCode)));
            }
        }
    }

}
