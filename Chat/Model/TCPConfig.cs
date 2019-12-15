using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chat.CommonModel
{
    public class TCPConfig
    {
        public string Name { get; set; }
        public IPAddress Address { get; set; }
        public int Port { get; set; } = 1454;
    }
}
