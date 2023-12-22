namespace GraphApp.Interactive.Commands
{
    internal class GraphPrintingCommand : ICommand
    {
        public string Name => "Print the contents of the current graph.";

        public bool RequiresGraph => false;

        public Task ExecuteAsync(CommandContext context)
        {
            if (context.CurrentGraphInstance == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("The graph hasn't been loaded.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Graph {context.CurrentGraphInstance.Name}");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                foreach (var node in context.CurrentGraphInstance.Nodes)
                {
                    Console.WriteLine(node);
                }
                Console.ResetColor();
            }
            return Task.CompletedTask;
        }
    }
}