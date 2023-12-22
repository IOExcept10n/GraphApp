using GraphApp.Graphs;
using Kurukuru;
using Sharprompt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GraphApp.Interactive.Commands
{
    internal class GraphDrawingCommand : ICommand
    {
        public string Name => "Draw a graph as PNG.";

        public async Task ExecuteAsync(CommandContext context)
        {
            string path = Prompt.Input<string>("File path", validators: [CommandExtensions.ValidatePath]);
            if (!path.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                path += ".png";
            Console.WriteLine("*Each frame will make graph less 'round', but if it has multiple connection groups, they will move away to the image corners instead.");
            int frame = Prompt.Input<int>("The animation frame", 1);
            int width = Prompt.Input<int>("The width of the image", 1024);
            int height = Prompt.Input<int>("The height of the image", 1024);
            var settings = CommandExtensions.RequestPaintSettings();
            await Spinner.StartAsync("Creating an image for the graph...", async spinner =>
            {
                var image = context.CurrentGraphInstance!.GetDefaultLayout(new Size(width, height), frame).DrawGraph<Argb32>(settings);
                await image.SaveAsPngAsync(path);
                spinner.Color = ConsoleColor.DarkGreen;
                spinner.Text = "Success!";
            });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Graph image {Path.GetFileName(path)} has been successfully saved.");
            Console.ResetColor();
        }
    }
}