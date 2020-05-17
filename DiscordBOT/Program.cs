using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace TestHoge
{
    class Program
    {
        private DiscordSocketClient _client;
        public static CommandService _commands;
        public static IServiceProvider _services;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });
            _client.Log += Log;
            _commands = new CommandService();
            _services = new ServiceCollection().BuildServiceProvider();
            _client.MessageReceived += CommandRecieved;
            //Discordtoken書く
            string token = "";
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        /// <summary>
        /// 何かしらのメッセージの受信
        /// </summary>
        /// <param name="msgParam"></param>
        /// <returns></returns>
        private async Task CommandRecieved(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            //デバッグ用メッセージを出力
            Console.WriteLine("{0} {1}:{2}", message.Channel.Name, message.Author.Username, message);
            //メッセージがnullの場合
            if (message == null)
                return;

            //発言者がBotの場合無視する
            if (message.Author.IsBot)
                return;


            var context = new CommandContext(_client, message);

            //ここから記述--------------------------------------------------------------------------
            var commandContext = message.Content;
            var s1 = commandContext.Substring(1,1);
            Console.WriteLine(s1);
            // メッセージ判定適当に書く
            if (s1 != "d") return;

            int dicecount;//diceの数
            int dicetype;// ダイスの種類
            Int32.TryParse(commandContext.Substring(0, 1), out dicecount);
            Int32.TryParse(commandContext.Substring(2), out dicetype);

            int result=0;

            for (int i = 0; i < dicecount; i++)
            {
                Dice.Throw(dicetype);
                result += Dice.Value;
            }

            {
                var str = "値は" + result;
                await message.Channel.SendMessageAsync(str);
            }
        }

        private Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
    // ダイス用クラス
    static class Dice
    {
        static readonly Random rmd;
        static public int Value { get; private set; }
        static Dice()
        {
            rmd = new Random(Environment.TickCount);
            Value = 1;
        }
        /// <summary>
        /// diceの種類で規則性のない数字出す
        /// </summary>
        /// <param name="dice"></param>
        static public void Throw(int dice)
        {
            Value = rmd.Next(dice) + 1;
        }
    }
}