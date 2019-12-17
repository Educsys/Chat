using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model
{
    /// <summary>
    /// Запись в логах сервера
    /// </summary>
    public class LogRecord
    {
        public DateTime LogTime { get; set; }
        public TypeOfLogRecord RecordType { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{LogTime.ToLocalTime()} {RecordType}: {Message}";
        }
    }

    public enum TypeOfLogRecord
    {
        Error,
        Info
    }
}
