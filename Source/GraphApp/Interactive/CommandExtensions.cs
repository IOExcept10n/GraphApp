using GraphApp.Graphs;
using Sharprompt;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using System.ComponentModel.DataAnnotations;

namespace GraphApp.Interactive
{
    internal static class CommandExtensions
    {
        public static Func<object, ValidationResult?> ValidatePath =>
            x => Uri.IsWellFormedUriString(x.ToString(), UriKind.RelativeOrAbsolute) ? ValidationResult.Success : new ValidationResult("The path is incorrect.");

        public static Func<object, ValidationResult?> FileExists =>
            x => File.Exists(x.ToString()) ? ValidationResult.Success : new ValidationResult("The file doesn't exist");

        public static GraphPaintSettings RequestPaintSettings()
        {
            GraphPaintSettings settings = new();
            bool change = Prompt.Confirm("Would you like to set up the graph drawing settings?", false);
            if (change)
            {
                var nodeColor = RequestColor("Input the color for the nodes");
                if (nodeColor != null)
                    settings.DefaultNodeBrush = new SolidBrush(nodeColor.Value);
                var edgeColor = RequestColor("Input the color for the edges");
                if (edgeColor != null)
                    settings.DefaultEdgePen = Pens.Solid(edgeColor.Value);
                bool textEnabled = Prompt.Confirm("Would you like to add the labels near edges and nodes?");
                settings.LabelsEnabled = textEnabled;
                if (textEnabled)
                {
                    string font = Prompt.Select<string>("Select the text font", from family in SystemFonts.Families select family.Name);
                    int size = Prompt.Input<int>("Input font size", 36);
                    settings.LabelFont = SystemFonts.CreateFont(font, size);
                }
                settings.NodeThickness = Prompt.Input<int>("Input the size of nodes", 8);
                settings.EdgeThickness = Prompt.Input<int>("Input the size of edges", 2);
            }
            return settings;
        }

        private static Color? RequestColor(string request)
        {
            if (Color.TryParse(Prompt.Input<string>(request), out var color))
            {
                return color;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Can't parse the color by name.");
                Console.ResetColor();
                return null;
            }
        }
    }
}