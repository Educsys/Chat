using Chat.Exceptions;
using Server.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Modules
{
    class UserManager
    {
        public ObservableCollection<ClientInfo> ConnectedClients = new ObservableCollection<ClientInfo>();


        public void AddUser(ClientInfo newclient)
        {
            if (ConnectedClients.Where(w => w.Name == newclient.Name).FirstOrDefault() != null)
                throw new UserAlreadyExistException("Пользователь с таким именем уже зарегистрирован");
            ConnectedClients.Add(newclient);
        }
    }
}
