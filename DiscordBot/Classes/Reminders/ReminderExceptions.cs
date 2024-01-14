using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Classes
{
    public class  ReminderNotFoundException : Exception
    {
        public ReminderNotFoundException() { }

        public ReminderNotFoundException(ulong id) : base( $"Reminder with ID {id} not found."){}

        public ReminderNotFoundException(ulong serverID, ulong id) : base($"Reminder with ID {id} not found on server {serverID}.") { }

    }
}
