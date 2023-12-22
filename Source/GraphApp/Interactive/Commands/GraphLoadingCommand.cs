using GraphApp.Graphs;
using Sharprompt;

namespace GraphApp.Interactive.Commands
{
    internal class GraphLoadingCommand : ICommand
    {
        public string Name => "Load a graph from the file.";

        public bool RequiresGraph => false;

        public Task ExecuteAsync(CommandContext context)
        {
            string path = Prompt.Input<string>("File path", validators: [CommandExtensions.ValidatePath, CommandExtensions.FileExists]);
            bool oriented = Prompt.Confirm(new ConfirmOptions()
            {
                Message = "Is graph oriented?",
                DefaultValue = false
            });
            using GraphReader reader = new(path);
            var graph = context.CurrentGraphInstance = reader.ReadGraph(oriented);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"The graph {graph.Name} has been successfully loaded. It has {graph.Count} nodes and {graph.Edges.Count()} edges.");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}