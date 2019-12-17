using System;
using System.ComponentModel;

namespace Chat.Model
{

    /// <summary>
    /// Сообщение клиент-клиент
    /// </summary>
    [Serializable]
    public class Message: INotifyPropertyChanged
    {
        public string Id { get; set; }
        public string SendTime { get; set; }
        public UserInfo Sender { get; set; }
        public UserInfo Recipient { get; set; }
        public string Text { get; set; }

        private MessageStatus status;

        public MessageStatus Status
        {
            get { return status; }
            set 
            { 
                status = value;
                OnPropertyChanded(nameof(Status));
            }
        }


        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanded(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public enum MessageStatus
    {
        OK,
        Error
    }
}
