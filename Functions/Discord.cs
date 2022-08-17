using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LIFTOFF.Functions
{
    class Discord
    {
        public static Discord discord = new Discord();

        public DiscordRpcClient client;

        public static Windows.Home home;

        public static Timestamps startTime = Timestamps.Now;

        public static bool DiscordAlive()
        {
            List<Process> discordProc = Process.GetProcesses()
                     .Where(x => x.ProcessName.ToLower().Contains("discord"))
                     .ToList();

            if (discordProc.Count > 0)
            {
                return true;
            }
            else return false;
        }

        public Discord()
        {
            client = new DiscordRpcClient("955496533703426138", -1, autoEvents: true, client: new DiscordRPC.IO.ManagedNamedPipeClient());
            client.SetSubscription(EventType.Join | EventType.JoinRequest);
            client.RegisterUriScheme();
            client.OnJoin += OnJoin;

            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            client.OnReady += (sender, e) =>
            {
                Core.Log("RPC Ready from user " + e.User.Username + " (" + e.User.ID + ")");
            };
            client.OnPresenceUpdate += (sender, e) =>
            {
                Core.Log("Presence Updated!");
            };
            client.OnConnectionFailed += (sender, e) =>
            {
                Core.Log("Unable to Connect to Discord!");

            };

            client.Initialize();
        }

        public static void JoinServer()
        {
            try
            {
                /*
                string tmpsecret = "Murderous Pursuits,90149646132336641";
                string[] secret = args.Secret.Split(',');
                string[] secret = tmpsecret.Split(',');
                var filteredGames = Variables.GameList.Where(game => game.Title == secret[0]).ToList();
                if (filteredGames.Any())
                {
                    var filteredServer = Variables.ServerList.Where(server => server.SteamID == secret[1]).ToList();
                    home.JoinServer(filteredServer.First(), true);
                }
                */
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        private static void OnJoin(object sender, JoinMessage args)
        {
            MessageBox.Show("EARLY TESTING MESSAGE: Request to join recieved from Discord, press okay to continue. (" + args.Secret + ")");
            try
            {
                string[] secret = args.Secret.Split(',');
                /*
                var filteredGames = Variables.GameList.Where(game => game.AppID.ToString() == secret[0]).ToList();
                if (filteredGames.Any())
                {
                    //var filteredServer = Variables.ServerList.Where(server => server.SteamID == secret[1]).ToList();
                    if (filteredServer.Any()) home.JoinServer(filteredServer.First(), true);
                    else MessageBox.Show("Cant find server. (Server ID: " + secret[1]);
                }
                else MessageBox.Show("It looks like you dont have that game :/ (App ID: " + secret[0]);
                */
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        /// <summary>
        /// Safely closes the RPC connection with Discord.
        /// </summary>
        public async Task Deinitialize()
        {
            await Core.Log("Closing Discord connection...");
            client.ClearPresence();
            client.Dispose();
            await Core.Log("Discord connection closed.");
        }
    }
}
