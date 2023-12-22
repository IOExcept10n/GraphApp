using GraphApp.Graphs;
using Kurukuru;
using Sharprompt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;

namespace GraphApp.Interactive.Commands
{
    internal class GraphAnimationDrawingCommand : ICommand
    {
        public string Name => "Draw the graph animation as GIF.";

        public async Task ExecuteAsync(CommandContext context)
        {
            string path = Prompt.Input<string>("File path", validators: [CommandExtensions.ValidatePath]);
            if (!path.EndsWith(".gif", StringComparison.InvariantCultureIgnoreCase))
                path += ".gif";
            int frames = Prompt.Input<int>("The animation frames number", 1);
            int width = Prompt.Input<int>("The width of the image", 1024);
            int height = Prompt.Input<int>("The height of the image", 1024);
            var settings = CommandExtensions.RequestPaintSettings();
            await Spinner.StartAsync("Creating an animation...", async spinner =>
            {
                Image<Rgb24> animation = new(width, height, Color.White);
                for (int i = 1; i < frames; i++)
                {
                    spinner.Text = $"Creating an animation (step {i} / {frames})...";
                    var image = context.CurrentGraphInstance!.GetDefaultLayout(new Size(width, height), i).DrawGraph<Rgb24>(settings, Color.White);
                    ImageFrame<Rgb24> frame = image.Frames[0];
                    ImageFrameMetadata metadata = frame.Metadata;
                    GifFrameMetadata frameMetadata = metadata.GetGifMetadata();
                    frameMetadata.DisposalMethod = GifDisposalMethod.RestoreToBackground;
                    animation.Frames.AddFrame(frame);
                }
                spinner.Text = "Saving image to the file...";
                await animation.SaveAsGifAsync(path);
                spinner.Color = ConsoleColor.DarkGreen;
                spinner.Text = "Success!";
            });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Animation {Path.GetFileName(path)} has been successfully saved.");
            Console.ResetColor();
        }
    }
}