using Chat.CommonModel;
using Chat.Model;
using Client.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Modules
{
    class ClientHandler : INotifyPropertyChanged
    {
        public ObservableCollection<HistoryUserInfo> UsersOnline { get; set; } = new ObservableCollection<HistoryUserInfo>();

        private HistoryUserInfo selectedUser;
        public HistoryUserInfo SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                if (SelectedUser != null)
                    SelectedUser.HasUnreadMessages = false;
                OnPropertyChanded(nameof(SelectedUser));
            }
        }

        private bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
            set 
            { 
                isConnected = value;
                OnPropertyChanded(nameof(IsConnected));
            }
        }

        public UserInfo MyUserInfo { get; set; }
        public TcpClient Client { get; set; }

        public void StartClient(TCPConfig clientconfig)
        {
            MyUserInfo = new UserInfo()
            {
                Name = clientconfig.Name
            };
            try
            {
                Client = new TcpClient(clientconfig.Address.ToString(), clientconfig.Port);
                SendDataToServer(TypeOfData.Connect, MyUserInfo);
                IsConnected = true;
                var receivedatatask = new Task(() => ReceivedDataType());
                receivedatatask.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void StopClient()
        {
            SendDataToServer(TypeOfData.Disconnect, (object)null);
            Disconnect();
        }

        private void Disconnect()
        {
            SelectedUser = null;
            App.Current.Dispatcher.Invoke(()=> UsersOnline.Clear());
            Client.Close();
            IsConnected = false;
        }

        public void SendMessageToUser(string text)
        {
            var message = new Message()
            {
                Sender = MyUserInfo,
                Recipient = new UserInfo() { Name = SelectedUser.Name, Id = SelectedUser.Id },
                Text = text
            };
            App.Current.Dispatcher.Invoke(() => SelectedUser.MessagesList.Add($"{DateTime.Now.ToLocalTime()} {MyUserInfo.Name}: {message.Text}"));
            SendDataToServer(TypeOfData.Message, message);
        }
        private void SendDataToServer<T>(TypeOfData typeOfData, T data)
        {
            try
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(Client.GetStream(), typeOfData);
                if (data != null)
                    formatter.Serialize(Client.GetStream(), data);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReceivedDataType()
        {
            var formatter = new BinaryFormatter();
            while (true)
            {
                try
                {
                    var typeofData = (TypeOfData)formatter.Deserialize(Client.GetStream());

                    switch (typeofData)
                    {
                        case TypeOfData.Connect:
                            MyUserInfo.Id = (string)formatter.Deserialize(Client.GetStream());
                            break;
                        case TypeOfData.Disconnect:
                            ReceiveDisconnect();
                            break;

                        case TypeOfData.Message:
                            ReceiveNewMessage();
                            break;
                        case TypeOfData.UserList:
                            ReceiveNewUserList();
                            break;
                        case TypeOfData.ERROR:
                            ReceiceErrorFromServer();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var test = ex;
                    break;
                }
            }
        }

        private T ReceiveDataFromServer<T>()
        {
            var formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(Client.GetStream());
        }

        private void ReceiceErrorFromServer()
        {
            var exception = ReceiveDataFromServer<Exception>();
            MessageBox.Show($"Ошибка отправки сообщения {exception.Message}");
        }

        private void ReceiveDisconnect()
        {
            Disconnect();
        }

        private void ReceiveNewMessage()
        {
            var message = ReceiveDataFromServer<Message>();
            var user = UsersOnline.Where(w => w.Id == message.Sender.Id).FirstOrDefault();
            App.Current.Dispatcher.Invoke(() => user.MessagesList.Add($"{DateTime.Now.ToLocalTime()} {user.Name}: {message.Text}"));
            if (SelectedUser != user)
                user.HasUnreadMessages = true;
        }
        private void ReceiveNewUserList()
        {
            var formatter = new BinaryFormatter();
            var users = ReceiveDataFromServer<UserInfo[]>().Where(w => w.Id != MyUserInfo.Id).ToList();

            foreach (var user in UsersOnline.Where(w => !users.Contains(w)).ToList())
            {
                App.Current.Dispatcher.Invoke(() => UsersOnline.Remove(user));
            }

            foreach (var user in users.Where(w => !UsersOnline.Contains(w)).ToList())
            {
                App.Current.Dispatcher.Invoke(() => UsersOnline.Add(new HistoryUserInfo(user)));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanded(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
