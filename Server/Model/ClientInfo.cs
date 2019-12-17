using Chat.Model;
using Server.Modules;
using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server.Model
{
    /// <summary>
    /// Хранит сущность подключенного клиента.
    /// Отвечает за прием данных от клиента и отправку данных серверу.
    /// </summary>
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
