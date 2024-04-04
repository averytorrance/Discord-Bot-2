using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DiscordBot;

namespace BotService
{
    public partial class DiscordBotService : ServiceBase
    {
        public DiscordBotService()
        {
            InitializeComponent();
        }

#if DEBUG
        /// <summary>
        /// Debug start
        /// </summary>
        public void OnStart() 
        {
            OnStart(null);
            while (true) ;
        } 
#endif

        protected override async void OnStart(string[] args)
        {
            await DiscordBot.Program.Main(args);
        }

        protected override void OnStop()
        {
        }
    }
}
