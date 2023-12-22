using GraphApp.Graphs;
using Kurukuru;
using Sharprompt;

namespace GraphApp.Interactive.Commands
{
    internal class GraphSolvingCommand : ICommand
    {
        public string Name => "Solve the graph tasks.";

        public Task ExecuteAsync(CommandContext context)
        {
            string destinationPath = Prompt.Input<string>("Destination file path", validators: [CommandExtensions.ValidatePath]);
            if (!destinationPath.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase))
                destinationPath += ".txt";
            Spinner.Start("Creating the task report...", spinner =>
            {
                using StreamWriter output = new(destinationPath);
                output.WriteLine($"Graph connectivity: {context.CurrentGraphInstance!.IsConnected()}");
                var bridges = context.CurrentGraphInstance!.GetBridges().ToList();
                output.WriteLine($"Graph bridges: {bridges.Count}");
                foreach (var bridge in bridges)
                {
                    output.WriteLine($"{bridge.Name}: {context.CurrentGraphInstance![bridge.SourceIndex].Name}-{context.CurrentGraphInstance![bridge.DestinationIndex].Name}");
                }
                spinner.Text = "Completed!";
            });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"The graph task has been successfully solved and saved to the file: {Path.GetFileName(destinationPath)}");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}