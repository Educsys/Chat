using Chat.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    class HistoryUserInfo: UserInfo, INotifyPropertyChanged
    {
        public HistoryUserInfo()
        {

        }
        public HistoryUserInfo(UserInfo user)
        {
            this.Id = user.Id;
            this.Name = user.Name;
        }

        private ObservableCollection<string> messagesList = new ObservableCollection<string>();
        public ObservableCollection<string> MessagesList
        {
            get { return messagesList; }
            set 
            { 
                messagesList = value;
                OnPropertyChanded(nameof(MessagesList));
            }
        }


        private bool hasUnreadMessages;
        public bool HasUnreadMessages
        {
            get { return hasUnreadMessages; }
            set
            {
                hasUnreadMessages = value;
                OnPropertyChanded(nameof(HasUnreadMessages));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanded(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
