using Discord;
using Discord.Interactions;
using System.Threading.Tasks;

namespace Robson_2._0.Commands
{
    public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("adicionar-atividade", "Adiciona uma atividade para a lista.")]
        public async Task AdicionarAtividadeCommand(
            [Summary(description: "O nome da matéria ou disciplina.")]
            string disciplina,

            [Summary(description: "A descrição da tarefa que precisa ser feita.")]
            string descricao,

            [Summary(description: "A data de entrega. Use o formato DD/MM/AAAA.")]
            string data,

            [Summary(description: "O link para a atividade ou material de apoio (opcional).")]
            string link = null)
        {
            // Sua lógica vai aqui...
            string resposta = $"Disciplina: **{disciplina}** Descrição: **{descricao}** Data: **{data}** Link: **{link}**";
            await RespondAsync(resposta, ephemeral: false);
        }

        [SlashCommand("ping", "Receba um pong de volta!")]
        public async Task PingCommand()
        {
            await RespondAsync("Pong!");
        }
    }
}