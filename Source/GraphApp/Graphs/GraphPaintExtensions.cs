using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace GraphApp.Graphs
{
    /// <summary>
    /// Contains the extension methods for the graph drawing.
    /// </summary>
    internal static class GraphPaintExtensions
    {
        /// <summary>
        /// Draws a graph using specified drawing settings.
        /// </summary>
        /// <typeparam name="TPixel">The type of the pixel used in the image.</typeparam>
        /// <param name="layout">The layout of the graph to draw.</param>
        /// <param name="settings">Settings for the drawing process.</param>
        /// <param name="background">The background color for an image.</param>
        /// <returns>The picture of the graph layout ready to save or send.</returns>
        public static Image<TPixel> DrawGraph<TPixel>(this GraphLayout layout, GraphPaintSettings? settings = null, TPixel? background = null) where TPixel : unmanaged, IPixel<TPixel>
        {
            settings ??= GraphPaintSettings.Default;
            Image<TPixel> result;
            if (background != null)
                result = new(layout.ImageSize.Width, layout.ImageSize.Height, background.Value);
            else result = new(layout.ImageSize.Width, layout.ImageSize.Height);
            result.Mutate(x =>
            {
                foreach (var node in layout.Graph.Nodes)
                {
                    // Draw the node
                    var nodeBrush = settings.NodeColorizer(node);
                    EllipsePolygon polygon = new(layout.VertexPositions[node.Index], settings.NodeThickness);
                    x.Fill(nodeBrush, polygon);
                    // Draw the labels right above the vertex.
                    x.DrawText(node.Name, settings.LabelFont, settings.TextForeground, settings.TextRound, layout.VertexPositions[node.Index] + Vector2.One * (settings.NodeThickness * 2));

                    // Draw the outcoming edges
                    foreach (var edge in node.OutcomingEdges)
                    {
                        var pen = settings.EdgeColorizer(edge);
                        var from = layout.VertexPositions[edge.SourceIndex];
                        var to = layout.VertexPositions[edge.DestinationIndex];
                        float distance = ((Vector2)to - (Vector2)from).Length();
                        // Source should start right after the vertex edge to not cross the arrow.
                        PointF source = Vector2.Lerp(from, to, settings.NodeThickness / distance);
                        // Destination should stop right before the vertex to make the arrow visible.
                        PointF destination = Vector2.Lerp(from, to, (distance - settings.NodeThickness) / distance);
                        x.DrawLine(pen, source, destination);

                        // Draw the labels
                        if (settings.LabelsEnabled)
                        {
                            // The point under the middle to draw text.
                            PointF middle = Vector2.Lerp(from, to, 0.5f) + Vector2.One * settings.NodeThickness;
                            x.DrawText(edge.Name, settings.LabelFont, settings.TextForeground, settings.TextRound, middle);
                        }
                    }
                }
            });
            return result;
        }
    }

    /// <summary>
    /// Provides the settings for the graph drawing process.
    /// </summary>
    internal class GraphPaintSettings
    {
        /// <summary>
        /// The default settings for the graph drawing.
        /// </summary>
        public static GraphPaintSettings Default { get; } = new();

        /// <summary>
        /// Gets or sets the size of the node for drawing.
        /// </summary>
        public float NodeThickness { get; set; } = 10;

        /// <summary>
        /// Gets or sets the width of the edge for drawing.
        /// </summary>
        public float EdgeThickness { get; set; } = 5;

        /// <summary>
        /// Gets or sets the default brush to colorize the nodes.
        /// </summary>
        public Brush DefaultNodeBrush { get; set; } = new SolidBrush(Color.Black);

        /// <summary>
        /// Gets or sets the default pen to colorize the edges.
        /// </summary>
        public Pen DefaultEdgePen { get; set; } = Pens.Solid(Color.Black);

        /// <summary>
        /// Gets or sets the outline color for the text.
        /// </summary>
        public Pen TextRound { get; set; } = Pens.Solid(Color.Black, 1);

        /// <summary>
        /// Gets or sets the color for the text.
        /// </summary>
        public Brush TextForeground { get; set; } = new SolidBrush(Color.White);

        /// <summary>
        /// Gets or sets the value that represents if the text labels for graph parts are enabled.
        /// </summary>
        public bool LabelsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the font to write graph parts labels.
        /// </summary>
        public Font LabelFont { get; set; } = new Font(SystemFonts.Get("Segoe UI"), 36);

        /// <summary>
        /// Gets or sets the function that selects the color for the node.
        /// </summary>
        public Func<Graph.GraphNode, Brush> NodeColorizer { get; set; }

        /// <summary>
        /// Gets or sets the function that selects the color of the edge.
        /// </summary>
        public Func<Graph.OrientedEdgeDefinition, Pen> EdgeColorizer { get; set; }

        public GraphPaintSettings()
        {
            NodeColorizer = DefaultNodeColorizer;
            EdgeColorizer = DefaultEdgeColorizer;
        }

        private Brush DefaultNodeColorizer(Graph.GraphNode _) => DefaultNodeBrush;

        private Pen DefaultEdgeColorizer(Graph.OrientedEdgeDefinition _) => DefaultEdgePen;
    }
}