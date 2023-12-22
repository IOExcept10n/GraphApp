using Sharprompt;

namespace GraphApp.Interactive.Commands
{
    internal class RemoveNodeCommand : ICommand
    {
        public string Name => "Remove a node from the graph.";

        public Task ExecuteAsync(CommandContext context)
        {
            string name = Prompt.Input<string>("Node name", validators: [Validators.Required()]);
            var first = context.CurrentGraphInstance!.Nodes.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (first != null)
            {
                int index = first.Index;
                context.CurrentGraphInstance!.RemoveNode(index);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"The node '{name}' has been successfully removed from the graph.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Can't find the node with specified name.");
                Console.ResetColor();
            }
            return Task.CompletedTask;
        }
    }
}