using Chat.Common;
using Chat.CommonModel;
using Server.Model;
using Server.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public IPAddress[] HostAddresses { get; set; }
        public TCPConfig ServerConfig { get; set; }

        private ServerHandler server;
        public ServerHandler Server
        {
            get { return server; }
            set
            {
                server = value;
                OnPropertyChanded(nameof(Server));
            }
        }

        public LogManager LogManager
        {
            get { return LogManager.Instance; }
        }

        #region Commands
        private RelayCommand startserver;
        public RelayCommand Startserver
        {
            get
            {
                return startserver ?? (startserver = new RelayCommand(obj =>
                {
                    Server.StartServer(ServerConfig);
                }, can => !Server.IsStarted
                            && !string.IsNullOrEmpty(ServerConfig.Name)
                            && !string.IsNullOrEmpty(ServerConfig.Port.ToString())
                            && !string.IsNullOrEmpty(ServerConfig.Address.ToString())));
            }
        }

        private RelayCommand stopserver;
        public RelayCommand Stopserver
        {
            get
            {
                return stopserver ?? (stopserver = new RelayCommand(obj =>
                {
                    Server.StopServer();
                }, can => Server.IsStarted));
            }
        }

        private RelayCommand disconnectSelectedUser;
        public RelayCommand DisconnectSelectedUser
        {
            get
            {
                return disconnectSelectedUser ?? (disconnectSelectedUser = new RelayCommand(obj =>
                {
                    if (Server.SelectedClient != null)
                        Server.DisconnectSelectedClient(Server.SelectedClient);
                }, can => Server.IsStarted));
            }
        }

        #endregion
        public MainWindowViewModel()
        {
            HostAddresses = Dns.GetHostAddresses(Dns.GetHostName()).Where(w => w.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToArray();
            ServerConfig = new TCPConfig();
            ServerConfig.Address = HostAddresses[0];
            Server = new ServerHandler();
        }

    }
}
