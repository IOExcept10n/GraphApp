using SixLabors.ImageSharp;

namespace GraphApp.Graphs
{
    /// <summary>
    /// Represents a layout of the graph.
    /// </summary>
    public class GraphLayout
    {
        /// <summary>
        /// Size of the image to place the layout.
        /// </summary>
        public Size ImageSize { get; init; }

        /// <summary>
        /// An array with the positions for each vertex of the graph.
        /// </summary>
        public Point[] VertexPositions { get; init; }

        /// <summary>
        /// The graph whose layout is represented.
        /// </summary>
        public Graph Graph { get; init; }

        internal GraphLayout(Point[] vertexPositions, Graph graph, Size imageSize)
        {
            if (vertexPositions.Length != graph.Count)
                throw new ArgumentException("Number of layout points and graph nodes should match.");
            VertexPositions = vertexPositions;
            Graph = graph;
            ImageSize = imageSize;
        }
    }
}