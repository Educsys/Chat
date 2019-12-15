using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Model
{
    [Serializable]
    public enum TypeOfData
    {
        Connect,
        Disconnect,
        Message,
        UserList,
        OK,
        ERROR
    }
}
