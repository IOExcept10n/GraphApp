using Sharprompt;

namespace GraphApp.Interactive.Commands
{
    internal class CreateGraphCommand : ICommand
    {
        public string Name => "Create an empty graph.";

        public bool RequiresGraph => false;

        public Task ExecuteAsync(CommandContext context)
        {
            string name = Prompt.Input<string>("Input the graph name");
            context.CurrentGraphInstance = new() { Name = name };
            return Task.CompletedTask;
        }
    }
}