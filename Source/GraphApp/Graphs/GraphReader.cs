using System.Globalization;

namespace GraphApp.Graphs
{
    /// <summary>
    /// Represents a class that can read graphs from the text stream source (e.g. text files).
    /// </summary>
    /// <remarks>
    /// For reading it uses the Node-Edge List (<see langword="NEL"/>) format of the file.
    /// </remarks>
    /// <example>
    /// The example format of the graph file written in <see langword="NEL"/> will look like that:
    /// Every element of the graph begins with the one-word token that determines the type of the element.
    /// First section contains nodes (nodes) of the graph
    /// Nodes' token is 'n', after that goes their index and then the name, split by spaces.
    /// It would be bad if the indices had too big numbers so I do not recommend doing that.
    /// The second section contains edges of the graph.
    /// Token of the edge is 'e', after it goes the pair of (source-destination) nodes and the name of the edge, split by spaces.
    /// The third is the graph token itself. It only contains a letter 'g' and the name of the graph.
    /// Also you can create comments using the '#' token on the line start.
    /// The file can have any other tokens but this program doesn't support it.
    /// </example>
    internal class GraphReader : IDisposable
    {
        private bool disposedValue;
        private readonly bool keepOpen;
        private readonly StreamReader reader;

        /// <inheritdoc cref="GraphReader(StreamReader)"/>
        /// <param name="filePath">The path to a file to read the graph from.</param>
        public GraphReader(string filePath) : this(new StreamReader(filePath))
        {
        }

        /// <inheritdoc cref="GraphReader(StreamReader, bool)"/>
        public GraphReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphReader"/> class.
        /// </summary>
        /// <param name="reader">The instance of the <see cref="StreamReader"/> to read the content of the file.</param>
        /// <param name="keepOpen">Determines whether to close the stream after disposing.</param>
        public GraphReader(StreamReader reader, bool keepOpen)
        {
            this.reader = reader;
            this.keepOpen = keepOpen;
        }

        ~GraphReader()
        {
            Dispose(disposing: false);
        }

        /// <summary>
        /// Reads the graph from the stream reader.
        /// </summary>
        /// <param name="oriented">
        /// An option to create oriented graph.
        /// If the graph is not oriented, each vertex will have the reversed copy (from destination to source) with the same name.
        /// </param>
        /// <returns>The instance of a <see cref="Graph"/> class with data from the file.</returns>
        /// <exception cref="FormatException">Occurs when the file contains unsupported data.</exception>
        public Graph ReadGraph(bool oriented = false)
        {
            List<Graph.GraphNode> nodes = new();
            // The dictionary to rename all indices to the ordinal form.
            Dictionary<int, int> renameMap = new();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var tokenType = GetTokenType(line);
                if (tokenType == ReaderTokenType.None || tokenType == ReaderTokenType.Comment)
                    continue;
                string[] token = line.Split();
                switch (tokenType)
                {
                    case ReaderTokenType.Node:
                        {
                            // The index will be ignored because the program should use indices as in an array anyway.
                            // If there would be empty places in the list, it will break the algorithms.
                            int index = int.Parse(token[1]);
                            renameMap.Add(index, nodes.Count);
                            string name = token[2];
                            nodes.Add(new Graph.GraphNode(name));
                        }
                        break;

                    case ReaderTokenType.Edge:
                        {
                            int from = renameMap[int.Parse(token[1])];
                            int to = renameMap[int.Parse(token[2])];
                            float weight = 1;
                            string name;
                            // The edge has the weight.
                            if (token.Length == 5)
                            {
                                weight = float.Parse(token[3], CultureInfo.InvariantCulture);
                                name = token[4];
                            }
                            else if (token.Length == 4)
                            {
                                name = token[3];
                            }
                            else throw new FormatException();
                            Graph.OrientedEdgeDefinition definition = new()
                            {
                                SourceIndex = from,
                                DestinationIndex = to,
                                Name = name,
                                Weight = weight,
                            };
                            nodes[from].OutcomingEdges.Add(definition);
                            // If the graph isn't oriented, we should add the back edge for proper work.
                            if (!oriented)
                            {
                                nodes[to].OutcomingEdges.Add(definition with { DestinationIndex = from, SourceIndex = to });
                            }
                        }
                        break;

                    case ReaderTokenType.Graph:
                        {
                            var graph = CreateGraph(nodes, token.Length >= 1 ? token[1] : null);
                            return graph;
                        }
                    default:
                        throw new FormatException("An unknown token while getting the graph info.");
                }
            }
            // In case when the graph wasn't specified.
            var namelessGraph = CreateGraph(nodes);
            return namelessGraph;
        }

        private static Graph CreateGraph(List<Graph.GraphNode> nodes, string? name = null)
        {
            var graph = new Graph();
            if (!string.IsNullOrEmpty(name)) graph.Name = name;
            foreach (var node in nodes)
            {
                if (node == null) break;
                graph.Add(node);
            }
            return graph;
        }

        private static ReaderTokenType GetTokenType(string line)
        {
            if (string.IsNullOrEmpty(line)) return ReaderTokenType.None;
            return char.ToLower(line[0]) switch
            {
                'n' => ReaderTokenType.Node,
                'e' => ReaderTokenType.Edge,
                'g' => ReaderTokenType.Graph,
                '#' => ReaderTokenType.Comment,
                _ => ReaderTokenType.Unknown
            };
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!keepOpen)
                    {
                        reader.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        private enum ReaderTokenType
        {
            None,
            Comment,
            Unknown,
            Node,
            Edge,
            Graph
        }
    }
}