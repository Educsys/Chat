using System;
using System.Linq;
using System.Threading;

namespace Server.Modules
{
    /// <summary>
    /// Класс таймера для периодической проверки неактивных пользователей.
    /// Отправляет команду серверу на отключение неактивных пользователей по таймеру.
    /// </summary>
    public class TimeoutManager
    {
        Timer timer;

        private readonly TimeSpan CHECK_PERIOD = new TimeSpan(0, 0, 2);
        private readonly TimeSpan MAXIMUM_TIMOUT_PERIOD = new TimeSpan(0, 1, 0);
        public TimeoutManager(ServerHandler server)
        {
            timer = new Timer(TimerElapsed, server, CHECK_PERIOD, CHECK_PERIOD);

        }

        private void TimerElapsed(object state)
        {
            var server = state as ServerHandler;
            var disconnectclients = server.Clients.Where(w => w.LastActivityTime.AddMilliseconds(MAXIMUM_TIMOUT_PERIOD.TotalMilliseconds) < DateTime.Now).ToList();
            foreach(var client in disconnectclients)
            {
                server.DisconnectSelectedClient(client);
            }
        }
    }
}
