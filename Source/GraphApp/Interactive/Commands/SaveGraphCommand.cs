using Sharprompt;

namespace GraphApp.Interactive.Commands
{
    internal class SaveGraphCommand : ICommand
    {
        public string Name => "Save graph into an interface";

        public Task ExecuteAsync(CommandContext context)
        {
            string filePath = Prompt.Input<string>("Input the file path.", validators: [CommandExtensions.ValidatePath]);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            using StreamWriter writer = new(filePath);
            foreach (var node in context.CurrentGraphInstance!.Nodes)
            {
                writer.WriteLine($"n {node.Index} {node.Name}");
            }
            foreach (var edge in context.CurrentGraphInstance!.Edges)
            {
                writer.WriteLine($"e {edge.SourceIndex} {edge.DestinationIndex} {edge.Name}");
            }
            writer.WriteLine($"g {context.CurrentGraphInstance!.Name}");
            return Task.CompletedTask;
        }
    }
}