using Chat.Model;
using Server.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model
{
    public class ClientInfo : UserInfo
    {
        public TcpClient Client { get; set; }
        public ServerHandler Server { get; set; }
        public DateTime LastActivityTime { get; set; }

        public ClientInfo(TcpClient client, ServerHandler server)
        {
            Client = client;
            Server = server;
            LastActivityTime = DateTime.Now;
        }

        public void ClientListen()
        {
            try
            {
                while (true)
                {
                    GetData();
                }
            }
            catch
            {

            }
        }

        public void GetData()
        {
            var formatter = new BinaryFormatter();
            var typeofData = (TypeOfData)formatter.Deserialize(Client.GetStream());

            LastActivityTime = DateTime.Now;

            switch (typeofData)
            {
                case TypeOfData.Connect:
                    {
                        var userinfo = (UserInfo)formatter.Deserialize(Client.GetStream());
                        Name = userinfo.Name;
                        Id = Guid.NewGuid().ToString();
                        Server.ConnectUser(this);
                        break;
                    }

                case TypeOfData.Disconnect:
                    {
                        Server.DisconnectByUserDecision(this);
                        break;
                    }
                case TypeOfData.Message:
                    {
                        var message = ReceiveDataFromClient<Message>();
                        Server.SendMessageToUser(this, message);
                        break;
                    }
            }
        }

        private T ReceiveDataFromClient<T>()
        {
            var formatter = new BinaryFormatter();
            T data = (T)formatter.Deserialize(Client.GetStream());
            return data;
        }
    }
}
