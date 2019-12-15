using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Model
{
    [Serializable]
    public class Message
    {
        public UserInfo Sender { get; set; }
        public UserInfo Recipient { get; set; }

        public string Text { get; set; }
    }
}
