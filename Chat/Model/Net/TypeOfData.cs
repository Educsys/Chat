using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Model
{
    /// <summary>
    /// Определяет тип данных, которые будут следовать далее.
    /// </summary>
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
