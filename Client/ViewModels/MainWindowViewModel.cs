using Chat.Common;
using Chat.CommonModel;
using Client.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    class MainWindowViewModel: ViewModelBase
    {
        public TCPConfig ClientConfig { get; set; }
        public IPAddress[] HostAddresses { get; set; }

        private ClientHandler client;
        public ClientHandler Client
        {
            get { return client; }
            set 
            {
                client = value;
                OnPropertyChanded(nameof(Client));
            }
        }

        private string messageText;

        public string MessageText
        {
            get { return messageText; }
            set 
            {
                messageText = value;
                OnPropertyChanded(nameof(MessageText));
            }
        }


        #region Commands
        private RelayCommand startclient;
        public RelayCommand Startclient
        {
            get
            {
                return startclient ?? (startclient = new RelayCommand(obj =>
                {
                    Client.StartClient(ClientConfig);
                }, can => !Client.IsConnected 
                            && !string.IsNullOrEmpty(ClientConfig.Name) 
                            && !string.IsNullOrEmpty(ClientConfig.Port.ToString())
                            && !string.IsNullOrEmpty(ClientConfig.Address.ToString())));
            }
        }

        private RelayCommand stopclient;
        public RelayCommand Stopclient
        {
            get
            {
                return stopclient ?? (stopclient = new RelayCommand(obj =>
                {
                    Client.StopClient();
                }, can => Client.IsConnected));
            }
        }


        private RelayCommand sendMessageToSelectedUser;
        public RelayCommand SendMessageToSelectedUser
        {
            get
            {
                return sendMessageToSelectedUser ?? (sendMessageToSelectedUser = new RelayCommand(obj =>
                {
                    if (string.IsNullOrEmpty(MessageText))
                        return;

                    Client.SendMessageToUser(MessageText);

                    MessageText = string.Empty;
                }, can => Client.IsConnected && Client.SelectedUser != null));
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            HostAddresses = Dns.GetHostAddresses(Dns.GetHostName()).Where(w => w.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToArray();
            ClientConfig = new TCPConfig();
            ClientConfig.Address = HostAddresses[0];
            Client = new ClientHandler();
        }

    }
}
