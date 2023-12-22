namespace GraphApp.Graphs
{
    /// <summary>
    /// An extension class that contains the task functions such as graph connectivity test, bridge test etc.
    /// </summary>
    internal static class GraphTesting
    {
        /// <summary>
        /// Gets the value that determines whether the graph is connected.
        /// </summary>
        /// <param name="graph">The graph to test.</param>
        /// <returns><see langword="true"/> if the graph is connected, <see langword="false"/> otherwise.</returns>
        public static bool IsConnected(this Graph graph)
        {
            return !graph.BreadthFirstSearch(0).Contains(-1);
        }

        /// <summary>
        /// Gets all the bridges of the graph.
        /// </summary>
        /// <remarks>
        /// Bridges are the edges that connects two parts that could've be different connectivity groups without the bridge.
        /// </remarks>
        /// <param name="graph">The graph to test.</param>
        /// <returns>All the bridges of the graph.</returns>
        public static IEnumerable<Graph.OrientedEdgeDefinition> GetBridges(this Graph graph)
        {
            // Copy the edges because their original contents will change while the algorithm work.
            var edges = graph.Edges.ToList();
            // Gets the amount of the connectivity groups in the graph.
            // Because they are numbered ordinally from 0, the maximal group + 1 will be the amount.
            int connectivityGroupsAmount = graph.GetConnectivityGroups().Max() + 1;
            foreach (var edge in edges)
            {
                // Try to remove the edge.
                graph.RemoveEdge(edge.SourceIndex, edge.DestinationIndex);
                // Check if the amount of connectivity groups has increased.
                int newGroups = graph.GetConnectivityGroups().Max() + 1;
                // Return the edge back on its place to not modify the source graph.
                graph.AddEdge(edge);
                // In that case, the edge is a bridge.
                if (newGroups > connectivityGroupsAmount)
                    yield return edge;
            }
        }

        ///// <summary>
        ///// Gets all the blocks in the graph.
        ///// </summary>
        ///// <param name="graph">The graph instance to test.</param>
        ///// <returns>
        ///// The array with numbered nodes. Number <see langword="0"/> means that the node is a connection point.
        ///// Any other number is a number of the block in the graph.
        ///// </returns>
        //public static int[] GetBlocks(this Graph graph)
        //{
        //    int[] results = new int[graph.Count];
        //    int timer = 0;
        //    int[] inputTime = new int[graph.Count];
        //    int[] upTime = new int[graph.Count];
        //    graph.DepthFirstSearch(0, n =>
        //    {
        //    });
        //}
    }
}