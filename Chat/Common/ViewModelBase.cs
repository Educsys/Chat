using System.ComponentModel;

namespace Chat.Common
{
    /// <summary>
    /// Базовый класс для вьюмоделей окон
    /// </summary>
    public class ViewModelBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanded(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
