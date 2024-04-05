using System;
using System.ServiceProcess;
using DiscordBot.Classes;

namespace BotService
{
    public partial class DiscordBotService : ServiceBase
    {
        public DiscordBotService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Debug start
        /// </summary>
        public void OnStart() 
        {
            OnStart(null);
            while (true) ;
        } 

        protected override void OnStart(string[] args)
        {
            Log.WriteToFile(Log.LogLevel.BotService, $"Service is started at {DateTime.Now}");
            StartBot(args);
        }

        private async void StartBot(string[] args)
        {
            await DiscordBot.Program.Main(args);
        }

        protected override void OnStop()
        {
            Log.WriteToFile(Log.LogLevel.BotService,$"Service is stopped at {DateTime.Now}");
        }


    }
}
