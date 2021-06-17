using System;
using System.Reflection;
using System.Threading.Tasks;
using BabelDatabase;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BabelBot
{
    public class Program
    {
        private readonly CommandService _commands = new CommandService();
        private readonly DiscordSocketClient _client = new DiscordSocketClient();

        private IServiceProvider _services;

        private static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new Program().Start().GetAwaiter().GetResult();
        }

        public async Task Start()
        {
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = configurationBuilder.Build();

            _client.Log += Log;

            var config = new BabelConfig();
            configurationBuilder.Build().GetSection("BabelConfig").Bind(config);

            _services = BuildServiceProvider();

            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();

            new CommandHandler(_commands, _client, _services);

            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: _services);

            foreach (var x in _commands.Commands)
            {
                Console.WriteLine("Command: " + x.Name.ToString());
            }

            new UpdateHandler(_client, _services);

            await Task.Delay(-1);
        }

        private Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        public IServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddOptions();

            // serviceCollection.Configure<BabelConfig>(Configuration.GetSection("BabelConfig")); // No worky :(
            serviceCollection.AddSingleton<IConfiguration>(Configuration); // Worky :)

            serviceCollection.AddDbContext<BabelContext>(ServiceLifetime.Transient);
            serviceCollection.AddSingleton<CommandHandler>();
            serviceCollection.AddSingleton<UpdateHandler>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
