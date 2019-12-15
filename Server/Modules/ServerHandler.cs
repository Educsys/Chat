using Chat.CommonModel;
using Chat.Model;
using Server.Model;
using Server.Modules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Server.Modules
{
    /// <summary>
    /// Главный серверный класс
    /// </summary>
    public class ServerHandler : INotifyPropertyChanged
    {
        private TimeoutManager timeoutManager { get; set; }
        public ObservableCollection<ClientInfo> Clients { get; set; } = new ObservableCollection<ClientInfo>();

        private ClientInfo selectedClient;
        public ClientInfo SelectedClient
        {
            get { return selectedClient; }
            set
            {
                selectedClient = value;
                OnPropertyChanded(nameof(SelectedClient));
            }
        }

        public bool IsStarted { get; set; }

        private TcpListener Listener;

        public ServerHandler()
        {
            timeoutManager = new TimeoutManager(this);
        }
        public void StartServer(TCPConfig config)
        {
            try
            {
                Listener = new TcpListener(config.Address, config.Port);
                Listener.Start();
                LogManager.Instance.AddNewLogRecord(TypeOfLogRecord.Info, $"Сервер запущен: {config.Address}:{config.Port}");
                IsStarted = true;
                var listentask = new Task(() => RunListener());
                listentask.Start();
            }
            catch (Exception ex)
            {
                LogManager.Instance.AddNewLogRecord(TypeOfLogRecord.Error, $"Ошибка сервера: {ex.Message}");
            }
        }
        private void RunListener()
        {
            if (IsStarted)
            {
                while (true)
                {
                    try
                    {
                        var client = Listener.AcceptTcpClient();
                        var clientinfo = new ClientInfo(client, this);
                        var clienttask = new Task(() => clientinfo.ClientListen());
                        clienttask.Start();
                    }
                    catch (Exception ex)
                    {
                        LogManager.Instance.AddNewLogRecord(TypeOfLogRecord.Error, $"Ошибка сервера: {ex.Message}");
                        break;
                    }
                }
            }
        }

        public void StopServer()
        {
            try
            {
                Listener.Stop();
                foreach (var client in Clients)
                {
                    client.Client.Close();
                }
                Clients.Clear();
                LogManager.Instance.AddNewLogRecord(TypeOfLogRecord.Info, $"Сервер остановлен");
                IsStarted = false;
            }
            catch (Exception ex)
            {
                LogManager.Instance.AddNewLogRecord(TypeOfLogRecord.Error, $"Ошибка сервера: {ex.Message}");
            }
        }

        public void ConnectUser(ClientInfo clientInfo)
        {
            App.Current.Dispatcher.Invoke(() => Clients.Add(clientInfo));
            LogManager.Instance.AddNewLogRecord(TypeOfLogRecord.Info, $"Новый пользователь: {clientInfo.Name}");
            SendDataToClient(null, clientInfo, TypeOfData.Connect, clientInfo.Id);
            var clients = Clients
                .Select(s =>
                    new UserInfo()
                    {
                        Id = s.Id,
                        Name = s.Name
                    }).ToArray();
            BroadcastNewClientsList();
        }
        public void DisconnectByUserDecision(ClientInfo clientInfo)
        {
            if (Clients.Contains(clientInfo))
            {
                App.Current.Dispatcher.Invoke(() => Clients.Remove(clientInfo));
                LogManager.Instance.AddNewLogRecord(TypeOfLogRecord.Info, $"Пользователь: {clientInfo.Name} отключен");
                BroadcastNewClientsList();
            }
        }

        public void DisconnectSelectedClient(ClientInfo clientInfo)
        {
            if (Clients.Contains(clientInfo))
            {
                LogManager.Instance.AddNewLogRecord(TypeOfLogRecord.Info, $"Пользователь: {clientInfo.Name} отключен");
                SendDataToClient(null, clientInfo, TypeOfData.Disconnect, (object)null);
                App.Current.Dispatcher.Invoke(() => Clients.Remove(clientInfo));
                BroadcastNewClientsList();
            }
        }
        public void BroadcastNewClientsList()
        {
            var clients = Clients
                .Select(s =>
                new UserInfo()
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToArray();

            foreach (var client in Clients)
            {
                SendDataToClient(null, client, TypeOfData.UserList, clients);
            }
        }

        public void SendMessageToUser(ClientInfo clientInfo, Message message)
        {
            var recipient = Clients.Where(w => w.Id == message.Recipient.Id).FirstOrDefault();
            var sender = Clients.Where(w => w.Id == message.Sender.Id).FirstOrDefault();
            SendDataToClient(sender, recipient, TypeOfData.Message, message);
            MessageHistoryManager.Instance.AppendNewString(message);
            LogManager.Instance.AddNewLogRecord(TypeOfLogRecord.Info, $" Сообщение {message.Sender.Name} пользователю {message.Recipient.Name}");
        }
        private void SendDataToClient<T>(ClientInfo sender, ClientInfo recipient, TypeOfData typeOfData, T data)
        {
            try
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(recipient.Client.GetStream(), typeOfData);
                if (data != null)
                    formatter.Serialize(recipient.Client.GetStream(), data);
                if (sender != null)
                    SendDataToClient(null, sender, TypeOfData.OK, (object)null);
            }
            catch (Exception ex)
            {
                LogManager.Instance.AddNewLogRecord(TypeOfLogRecord.Error, $"Ошибка отправки данных: {ex.Message}");
                if (sender != null)
                    SendDataToClient(null, sender, TypeOfData.ERROR, ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanded(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
