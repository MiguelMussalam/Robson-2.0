using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using Robson_2._0.Commands;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public class Program
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _config;
    private readonly InteractionService _interactions;

    static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
    public Program()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var clientConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(clientConfig);
        _commands = new CommandService();
        _interactions = new InteractionService(_client.Rest);

        _services = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton(_config)
            .AddSingleton(_interactions)
            .BuildServiceProvider();
    }

    public async Task RunBotAsync()
    {
        _client.Log += log =>
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        };

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        await _client.LoginAsync(TokenType.Bot, _config["token"]);

        await _client.StartAsync();

        await Task.Delay(-1);
    }
}