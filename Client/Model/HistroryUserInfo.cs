using Chat.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Client.Model
{
    /// <summary>
    /// Дополнение к классу содержащему базовую информация о клиент.
    /// Содержит историю общения с данным клиентом.
    /// </summary>
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

        private ObservableCollection<Message> messagesList = new ObservableCollection<Message>();
        public ObservableCollection<Message> MessagesList
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
