using Chat.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Modules
{
    public class MessageHistoryManager
    {
        private const string HISTORY_FILE_NAME = "History.txt";
        private FileInfo historyFile;
        private MessageHistoryManager()
        {
            historyFile = new FileInfo(HISTORY_FILE_NAME);
        }

        private static MessageHistoryManager instance;
        public static MessageHistoryManager Instance
        {
            get
            {
                return instance ?? (instance = new MessageHistoryManager());
            }
        }

        public void AppendNewString(Message message)
        {
            try
            {
                File.AppendAllText(historyFile.FullName, $"{DateTime.Now.ToLocalTime()} {message.Sender.Name} - {message.Recipient.Name}: {message.Text} {Environment.NewLine}" );
            }
            catch(Exception ex)
            {
                LogManager.Instance.AddNewLogRecord(Model.TypeOfLogRecord.Error, ex.Message);
            }
        }
    }
}
