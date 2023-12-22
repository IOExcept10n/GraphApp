using Sharprompt;

namespace GraphApp.Interactive.Commands
{
    internal class RemoveEdgeCommand : ICommand
    {
        public string Name => "Remove the edge from the graph.";

        public Task ExecuteAsync(CommandContext context)
        {
            string fromName = Prompt.Input<string>("Name of the source node");
            string toName = Prompt.Input<string>("Name of the destination node");
            var fromNode = context.CurrentGraphInstance!.Nodes.FirstOrDefault(x => x.Name.Equals(fromName, StringComparison.InvariantCultureIgnoreCase));
            var toNode = context.CurrentGraphInstance!.Nodes.FirstOrDefault(x => x.Name.Equals(toName, StringComparison.InvariantCultureIgnoreCase));
            if (fromNode != null && toNode != null)
            {
                int from = fromNode.Index, to = toNode.Index;
                if (context.CurrentGraphInstance!.RemoveEdge(from, to))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"The edge has been successfully removed from the graph.");
                    Console.ResetColor();
                    return Task.CompletedTask;
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Can't find the node(s) with specified name(s).");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}