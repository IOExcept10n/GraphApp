using Sharprompt;

namespace GraphApp.Interactive.Commands
{
    internal class AddEdgeCommand : ICommand
    {
        public string Name => "Add an edge to the graph.";

        public Task ExecuteAsync(CommandContext context)
        {
            string fromName = Prompt.Input<string>("Name of the source node");
            string toName = Prompt.Input<string>("Name of the destination node");
            var fromNode = context.CurrentGraphInstance!.Nodes.FirstOrDefault(x => x.Name.Equals(fromName, StringComparison.InvariantCultureIgnoreCase));
            var toNode = context.CurrentGraphInstance!.Nodes.FirstOrDefault(x => x.Name.Equals(toName, StringComparison.InvariantCultureIgnoreCase));
            if (fromNode != null && toNode != null)
            {
                int from = fromNode.Index, to = toNode.Index;
                string name = Prompt.Input<string>("Edge name");
                context.CurrentGraphInstance!.AddEdge(from, to, name);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"The edge '{name}' has been successfully added to the graph.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Can't find the node(s) with specified name(s).");
                Console.ResetColor();
            }
            return Task.CompletedTask;
        }
    }
}