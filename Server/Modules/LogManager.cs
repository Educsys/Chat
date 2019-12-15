using Server.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Modules
{
    /// <summary>
    /// Класс для отображения лога
    /// </summary>
   public class LogManager : INotifyPropertyChanged
    {
        private LogManager()
        {
            logRecords = new ObservableCollection<LogRecord>();
        }

        private static LogManager instance;
        public static LogManager Instance
        {
            get
            {
                return instance ?? (instance = new LogManager());
            }
        }

        private ObservableCollection<LogRecord> logRecords;

        public ObservableCollection<LogRecord> LogRecords
        {
            get { return logRecords; }
            private set { logRecords = value; }
        }

        public void AddNewLogRecord(TypeOfLogRecord type, string message)
        {
            var record = new LogRecord()
            {
                LogTime = DateTime.Now,
                RecordType = type,
                Message = message
            };
            App.Current.Dispatcher.Invoke(()=> LogRecords.Add(record));
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
