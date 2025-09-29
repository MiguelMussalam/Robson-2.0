// Program.cs

using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; // Para ler o appsettings.json

public class Program
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _config;
    
    // O Discord.Net usa um padrão de inicialização assíncrono
    static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

    public Program()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Configuração do cliente, incluindo os Gateway Intents
        var clientConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent // Habilita a leitura de mensagens
        };

        _client = new DiscordSocketClient(clientConfig);
        _commands = new CommandService();

        // Configuração da Injeção de Dependência
        _services = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton(_config)
            // Adicione outros serviços aqui se precisar
            .BuildServiceProvider();
    }

    public async Task RunBotAsync()
    {
        // Evento de log
        _client.Log += log =>
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        };

        // Evento que dispara quando uma mensagem é recebida
        _client.MessageReceived += HandleCommandAsync;

        // Descobre e carrega todos os módulos de comando do nosso projeto
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        // Faz login no Discord usando o token do arquivo de configuração
        await _client.LoginAsync(TokenType.Bot, _config["token"]);

        // Inicia a conexão com o Discord
        await _client.StartAsync();

        // Mantém o programa rodando indefinidamente
        await Task.Delay(-1);
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        // Garante que a mensagem não é do sistema ou de outro bot
        var message = arg as SocketUserMessage;
        if (message is null || message.Author.IsBot) return;

        int argPos = 0;

        // Verifica se a mensagem tem o prefixo "!" ou se menciona o bot
        if (message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
        {
            Console.WriteLine($"Comando recebido de {message.Author.Username}: {message.Content}");
            // Cria o contexto do comando
            var context = new SocketCommandContext(_client, message);

            // Tenta executar o comando
            await _commands.ExecuteAsync(context, argPos, _services);
        }
    }
}