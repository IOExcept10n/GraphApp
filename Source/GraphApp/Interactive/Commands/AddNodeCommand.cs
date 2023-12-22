using Sharprompt;

namespace GraphApp.Interactive.Commands
{
    internal class AddNodeCommand : ICommand
    {
        public string Name => "Add a node to the graph.";

        public Task ExecuteAsync(CommandContext context)
        {
            string name = Prompt.Input<string>("Node name", validators: [Validators.Required()]);
            context.CurrentGraphInstance!.AddNode(name);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"The node '{name}' has been successfully added to the graph.");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}