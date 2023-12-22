using SixLabors.ImageSharp;
using System.Numerics;

namespace GraphApp.Graphs
{
    /// <summary>
    /// Represents a data structure that represents an oriented graph that can store any data inside.
    /// </summary>
    public class Graph : IIdentifiable
    {
        private readonly List<GraphNode> nodes = new();

        /// <summary>
        /// Gets a number of nodes in the graph.
        /// </summary>
        public int Count => nodes.Count;

        /// <inheritdoc/>
        public string Name { get; set; } = "G";

        /// <summary>
        /// Gets the node of the graph at specified index.
        /// </summary>
        /// <param name="index">An index to get the node.</param>
        /// <returns>The node at the specified index.</returns>
        public GraphNode this[int index] { get => nodes[index]; }

        /// <summary>
        /// Gets all the edges defined in graph, ordered by source nodes' indices.
        /// </summary>
        public IEnumerable<OrientedEdgeDefinition> Edges
        {
            get
            {
                foreach (var node in nodes)
                {
                    foreach (var edge in node.OutcomingEdges)
                    {
                        yield return edge;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the list view of all the graph nodes.
        /// </summary>
        public IReadOnlyList<GraphNode> Nodes => nodes;

        /// <summary>
        /// Creates a new instance of the <see cref="Graph"/> class.
        /// </summary>
        public Graph()
        {
        }

        /// <inheritdoc cref="Graph()"/>
        /// <param name="nodes">The list of nodes to set.</param>
        public Graph(List<GraphNode> nodes)
        {
            // To create a copy of the original list.
            this.nodes = [.. nodes];
        }

        /// <summary>
        /// Performs the Depth-First Search (<see langword="DFS"/>) algorithm in the graph, starting from given node, using custom <paramref name="predicate"/> function.
        /// </summary>
        /// <remarks>
        /// The basic algorithm doesn't provide checks for double-passing into a single node, so you should check for cycles in the graph by yourself.
        /// </remarks>
        /// <param name="index">The index of a node to start the algorithm.</param>
        /// <param name="predicate">
        /// The function to test if a node should be iterated on the next step of the algorithm.
        /// If the call returns <see langword="false"/>, this step should end.
        /// </param>
        /// <returns>
        /// The enumeration with all the nodes that passes the condition of the <paramref name="predicate"/>
        /// and can be accessed from the node at given <paramref name="index"/>,
        /// or the empty <see cref="Enumerable"/> if the search didn't provide any node.
        /// </returns>
        public IEnumerable<GraphNode> DepthFirstSearch(int index, Func<GraphNode, bool> predicate)
        {
            var node = this[index];
            if (predicate(node))
            {
                IEnumerable<GraphNode> result = [node];
                foreach (var edge in node.OutcomingEdges)
                {
                    result = result.Concat(DepthFirstSearch(edge.DestinationIndex, predicate));
                }
                return result;
            }
            return Enumerable.Empty<GraphNode>();
        }

        /// <inheritdoc cref="BreadthFirstSearch(int)"/>
        /// <param name="startIndex">The starting index of the beginning node.</param>
        /// <param name="predicate">
        /// An optional predicate for the search in the case
        /// if the search will require only nodes that passes any custom condition.
        /// </param>
        public int[] BreadthFirstSearch(int startIndex, Func<GraphNode, bool>? predicate = null)
        {
            int[] distances = new int[Count];
            // Mark all nodes as inaccessible until they'll be accessed.
            Array.Fill(distances, -1);
            distances[startIndex] = 0;

            // Create a queue for all available nodes to iterate through them.
            Queue<int> accessibleNodes = new();
            accessibleNodes.Enqueue(startIndex);

            while (accessibleNodes.Count != 0)
            {
                int index = accessibleNodes.Dequeue();
                var node = nodes[index];
                // Skip the node if it doesn't match the predicate.
                if (predicate?.Invoke(node) == false)
                    continue;
                foreach (var edge in node.OutcomingEdges)
                {
                    // If the edge at given index wasn't visited but can be visited now, mark it.
                    if (distances[edge.DestinationIndex] == -1)
                    {
                        distances[edge.DestinationIndex] = distances[index] + 1;
                        accessibleNodes.Enqueue(edge.DestinationIndex);
                    }
                }
            }

            return distances;
        }

        /// <summary>
        /// Performs a Breadth-First Search (<see langword="BFS"/>) algorithm for the graph to find a path from the one node to another.
        /// </summary>
        /// <remarks>
        /// Note that this method doesn't count the distance using edge weights, it finds only an accessibility to the node
        /// and the distance is the amount of nodes to arrive to the destination.
        /// </remarks>
        /// <param name="startIndex">The index of the beginning node.</param>
        /// <returns>
        /// An array with the distances to each node accessible from the beginning node.
        /// Each value of the resulting array will match the node at the same index.
        /// </returns>
        public int[] BreadthFirstSearch(int startIndex)
        {
            int[] distances = new int[Count];
            // Mark all nodes as inaccessible until they'll be accessed.
            Array.Fill(distances, -1);
            distances[startIndex] = 0;

            // Create a queue for all available nodes to iterate through them.
            Queue<int> accessibleNodes = new();
            accessibleNodes.Enqueue(startIndex);

            while (accessibleNodes.Count != 0)
            {
                int index = accessibleNodes.Dequeue();
                var node = nodes[index];
                foreach (var edge in node.OutcomingEdges)
                {
                    // If the edge at given index wasn't visited but can be visited now, mark it.
                    if (distances[edge.DestinationIndex] == -1)
                    {
                        distances[edge.DestinationIndex] = distances[index] + 1;
                        accessibleNodes.Enqueue(edge.DestinationIndex);
                    }
                }
            }

            return distances;
        }

        /// <summary>
        /// Gets the distance from the one node to another.
        /// </summary>
        /// <param name="startIndex">The source of the path.</param>
        /// <param name="endIndex">The destination of the path.</param>
        /// <returns>The distance to the destination node measured as
        /// the edges' weights sum of the shortest path to the destination
        /// or <see cref="float.PositiveInfinity"/> if there are no paths to the destination.</returns>
        public float MinimalDistance(int startIndex, int endIndex)
        {
            float[] distances = new float[Count];
            // Mark all nodes as inaccessible until they'll be accessed.
            Array.Fill(distances, float.PositiveInfinity);
            distances[startIndex] = 0;

            // Create a queue for all available nodes to iterate through them.
            Queue<int> accessibleNodes = new();
            accessibleNodes.Enqueue(startIndex);

            while (accessibleNodes.Count != 0)
            {
                int index = accessibleNodes.Dequeue();
                var node = nodes[index];
                foreach (var edge in node.OutcomingEdges)
                {
                    // If the edge at given index wasn't visited but can be visited now, mark it.
                    if (distances[edge.DestinationIndex] == float.PositiveInfinity)
                    {
                        distances[edge.DestinationIndex] = distances[index] + edge.Weight;
                        accessibleNodes.Enqueue(edge.DestinationIndex);
                    }
                }
            }

            return distances[endIndex];
        }

        /// <summary>
        /// Computes the connectivity groups for the graph.
        /// </summary>
        /// <returns>An array with numbers of the connectivity group for every graph node.</returns>
        public int[] GetConnectivityGroups()
        {
            int[] groups = new int[Count];
            Array.Fill(groups, -1);
            int group = 0;
            for (int i = 0; i < Count; i++)
            {
                if (groups[i] == -1)
                {
                    group++;
                    DepthFirstSearch(i, x =>
                    {
                        if (groups[x.Index] == -1)
                        {
                            groups[x.Index] = group;
                            return true;
                        }
                        return false;
                    });
                }
            }
            return groups;
        }

        /// <summary>
        /// Gets the default layout for the graph.
        /// </summary>
        /// <param name="imageSize">Size of an image to scale the layout.</param>
        /// <param name="animationFrame">The iteration of the graph placing animation.</param>
        /// <returns>
        /// The default layout of the graph.
        /// It's based on the circle layout but every animation step will try to move nodes according to their connections.
        /// So on every animation step the vertices will move away from each other, then every edge will bring them closer according to its weight.
        /// </returns>
        public GraphLayout GetDefaultLayout(Size imageSize, int animationFrame = 1)
        {
            var unscaledPositions = GetDefaultUnscaledLayout(animationFrame);
            Point[] scaledPositions = ScalePositions(imageSize, unscaledPositions, imageSize / 8);
            return new GraphLayout(scaledPositions, this, imageSize);
        }

        private static Point[] ScalePositions(Size imageSize, Vector2[] unscaledPositions, Size padding)
        {
            if (unscaledPositions.Length == 0)
                return Array.Empty<Point>();
            imageSize -= 2 * padding;
            Point[] result = new Point[unscaledPositions.Length];
            // Prepare data about minimal and maximal coordinates.
            float minX = unscaledPositions[0].X, maxX = unscaledPositions[0].X,
                minY = unscaledPositions[0].Y, maxY = unscaledPositions[0].Y;
            for (int i = 0; i < result.Length; i++)
            {
                var pos = unscaledPositions[i];
                minX = float.Min(minX, pos.X);
                minY = float.Min(minY, pos.Y);
                maxX = float.Max(maxX, pos.X);
                maxY = float.Max(maxY, pos.Y);
            }

            // The bounds of the unscaled image to scale.
            Vector2 difference = new(maxX - minX, maxY - minY);
            Vector2 scale = new(imageSize.Width / difference.X, imageSize.Height / difference.Y);

            // The minimal coordinates of the graph to move.
            Vector2 begin = new Vector2(minX, minY) * scale;
            // The maximal coordinates of the graph to move.
            Vector2 end = new Vector2(maxX, maxY) * scale;
            // The middle of the graph.
            Vector2 middle = Vector2.Lerp(begin, end, 0.5f);
            // The middle of an image.
            int imageMiddleX = imageSize.Width / 2 + padding.Width, imageMiddleY = imageSize.Height / 2 + padding.Height;
            // The offset between them to move each graph vertex.
            Vector2 offset = new Vector2(imageMiddleX, imageMiddleY) - middle;

            for (int i = 0; i < unscaledPositions.Length; i++)
            {
                // Step 1: scale
                unscaledPositions[i] *= scale;
                // Step 2: move
                unscaledPositions[i] += offset;
                // Step 3: write a point.
                result[i] = new Point((int)unscaledPositions[i].X, (int)unscaledPositions[i].Y);
            }
            return result;
        }

        private Vector2[] GetDefaultUnscaledLayout(int iterations)
        {
            // Create an unscaled graph image.
            Vector2[] positions = new Vector2[Count];
            Vector2[] displacements = new Vector2[Count];
            // Initialize node positions on a circle.
            float a = 0, da = 2 * MathF.PI / Count;
            for (int i = 0; i < Count; i++)
            {
                positions[i] = new Vector2(Count * MathF.Sin(a), Count * MathF.Cos(a));
                a += da;
            }

            // Initialize parameters for moving. Area can be anything, it's just a coefficient.
            float area = Count;
            float k2 = area / Count, k = MathF.Sqrt(k2);

            for (int i = 0; i < iterations; i++)
            {
                // Temperature cools down smoothly, any other formula for change can be used.
                float temperature = 1 - i / (float)iterations;
                temperature *= temperature;

                // Calculate repulsive forces
                for (int n1 = 0; n1 < Count; n1++)
                {
                    for (int n2 = 0; n2 < Count; n2++)
                    {
                        if (n1 == n2) continue;
                        Vector2 distance = positions[n1] - positions[n2];
                        float coefficient = k2 / distance.LengthSquared();
                        displacements[n1] += distance * coefficient;
                    }
                }

                // Calculate attractive forces
                foreach (var edge in Edges)
                {
                    Vector2 distance = positions[edge.SourceIndex] - positions[edge.DestinationIndex];
                    float coefficient = distance.Length() / k * edge.Weight;
                    displacements[edge.SourceIndex] -= distance * coefficient;
                    displacements[edge.DestinationIndex] += distance * coefficient;
                }

                // Calculate positions
                float sum = 0;
                for (int n = 0; n < Count; n++)
                {
                    float d = displacements[n].Length();
                    if (d > temperature)
                    {
                        float coefficient = temperature / d;
                        displacements[n] *= coefficient;
                        sum += temperature;
                    }
                    else sum += d;
                    positions[n] += displacements[n];
                }
            }
            return positions;
        }

        /// <summary>
        /// Adds a node to a graph.
        /// </summary>
        /// <param name="name">Name of a node to add.</param>
        /// <returns>An instance of a created node.</returns>
        public GraphNode AddNode(string name)
        {
            GraphNode newNode = new(name);
            Add(newNode);
            return newNode;
        }

        /// <summary>
        /// Adds an edge to the graph.
        /// </summary>
        /// <param name="edge">The edge definition to add.</param>
        public void AddEdge(OrientedEdgeDefinition edge)
        {
            AssertEdge(edge);
            var node = nodes[edge.SourceIndex];
            node.OutcomingEdges.Add(edge);
        }

        /// <summary>
        /// Adds an edge between two nodes in a graph.
        /// </summary>
        /// <param name="from">The source node.</param>
        /// <param name="to">The destination node.</param>
        /// <param name="name">The name of an edge. If the name is <see langword="null"/>, the next ordinal number of the edge will be used.</param>
        /// <returns></returns>
        public OrientedEdgeDefinition AddEdge(int from, int to, string? name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = $"v{Edges.Count()}";
            }
            var node = nodes[from];
            var definition = new OrientedEdgeDefinition()
            {
                SourceIndex = from,
                DestinationIndex = to,
                Name = name,
                Weight = 1
            };
            AssertEdge(definition);
            node.OutcomingEdges.Add(definition);
            return definition;
        }

        /// <summary>
        /// Removes the edge in the graph.
        /// </summary>
        /// <param name="from">The starting index of an edge.</param>
        /// <param name="to">The ending index of an edge.</param>
        /// <returns><see langword="true"/> if the edge was found and removed, <see langword="false"/> otherwise.</returns>
        public bool RemoveEdge(int from, int to)
        {
            var node = nodes[from];
            var edge = node.OutcomingEdges.FirstOrDefault(x => x.DestinationIndex == to);
            if (edge != default)
            {
                node.OutcomingEdges.Remove(edge);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the node at the given index.
        /// </summary>
        /// <param name="index">The index of the node to remove.</param>
        /// <returns><see langword="true"/> if the node was successfully removed, <see langword="false"/> otherwise.</returns>
        public bool RemoveNode(int index)
        {
            if (index >= nodes.Count) return false;
            foreach (var node in nodes)
            {
                for (int i = 0; i < node.OutcomingEdges.Count; i++)
                {
                    OrientedEdgeDefinition edge = node.OutcomingEdges[i];
                    if (edge.DestinationIndex == index)
                    {
                        // Remove all the edges that targets the current node.
                        node.OutcomingEdges.RemoveAt(i--);
                    }
                    else if (edge.DestinationIndex > index)
                    {
                        // Reduces the index of all other edges if it's more than the index of the removed node.
                        node.OutcomingEdges[i] = edge with { DestinationIndex = edge.DestinationIndex - 1 };
                    }
                }
                // Move the nodes' indices because the removal will move them.
                if (node.Index > index)
                {
                    node.Index--;
                }
            }
            nodes.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Adds a node to a graph.
        /// </summary>
        /// <param name="item">A node to bind to a graph.</param>
        internal void Add(GraphNode item)
        {
            item.Index = nodes.Count;
            nodes.Add(item);
            item.ParentGraph = this;
        }

        private void AssertEdge(OrientedEdgeDefinition definition)
        {
            if (definition.SourceIndex < 0 || definition.DestinationIndex >= nodes.Count)
                throw new ArgumentOutOfRangeException(nameof(definition), "The indices of the graph nodes should be included in the graph.");
        }

        /// <summary>
        /// Represents a definition of an edge in the graph.
        /// </summary>
        public readonly record struct OrientedEdgeDefinition : IIdentifiable
        {
            /// <inheritdoc/>
            public string Name { get; init; }

            /// <summary>
            /// The weight of an edge.
            /// </summary>
            public float Weight { get; init; }

            /// <summary>
            /// The index of a source of an oriented edge.
            /// </summary>
            public int SourceIndex { get; init; }

            /// <summary>
            /// The index of a target of an oriented edge.
            /// </summary>
            public int DestinationIndex { get; init; }

            /// <inheritdoc/>
            public override string ToString()
            {
                return $"{Name}: {SourceIndex}-{DestinationIndex}";
            }
        }

        /// <summary>
        /// Represents a node of the graph.
        /// </summary>
        public class GraphNode : IIdentifiable
        {
            /// <summary>
            /// Gets the graph that owes the node.
            /// </summary>
            public Graph? ParentGraph { get; internal set; }

            /// <inheritdoc/>
            public string Name { get; set; }

            /// <summary>
            /// Gets an index of a node.
            /// </summary>
            public int Index { get; internal set; }

            /// <summary>
            /// Gets all the edges that come to the current node.
            /// </summary>
            public IEnumerable<OrientedEdgeDefinition> IncomingEdges => ParentGraph == null ?
                    Enumerable.Empty<OrientedEdgeDefinition>() :
                    from node in ParentGraph.nodes
                    let edge = node.OutcomingEdges.FirstOrDefault(e => e.DestinationIndex == Index)
                    where edge != default
                    select edge;

            /// <summary>
            /// List of incident edges that begins in this node.
            /// </summary>
            public List<OrientedEdgeDefinition> OutcomingEdges { get; init; } = new();

            /// <summary>
            /// Creates a node with the name, edges and the optional data.
            /// </summary>
            /// <param name="name">The name of a node.</param>
            public GraphNode(string name)
            {
                Name = name;
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return $"{Name}: [{string.Join(',', from edge in OutcomingEdges select ParentGraph?[edge.DestinationIndex].Name)}]";
            }
        }
    }
}