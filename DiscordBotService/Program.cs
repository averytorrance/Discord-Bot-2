using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BotService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //DiscordBotService service = new DiscordBotService();
            //service.OnStart();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new DiscordBotService()
            };
            ServiceBase.Run(ServicesToRun);

        }
    }
}
