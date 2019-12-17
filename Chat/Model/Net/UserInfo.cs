using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Model
{
    /// <summary>
    /// Содержит базовую часть информации о чат клиенте для пересылки между сервером и клиентами.
    /// </summary>
    [Serializable]
    public class UserInfo
    { 
        public string Name { get; set; }
        public string Id { get; set; }

    }
}
