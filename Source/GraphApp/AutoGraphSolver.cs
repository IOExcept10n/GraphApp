using GraphApp.Graphs;

namespace GraphApp
{
    internal static class AutoGraphSolver
    {
        /// <summary>
        /// Solve the tasks for the graph.
        /// </summary>
        /// <param name="sourceFile">The source file that contains the graph info.</param>
        /// <param name="destinationFile">The destination file that will contain the tasks report for the graph.</param>
        public static void SolveGraph(string sourceFile, string destinationFile)
        {
            using GraphReader reader = new(sourceFile);
            var graph = reader.ReadGraph();
            using StreamWriter output = new(destinationFile);
            output.WriteLine($"Graph connectivity: {graph.IsConnected()}");
            var bridges = graph.GetBridges().ToList();
            output.WriteLine($"Graph bridges: {bridges.Count}");
            foreach (var bridge in bridges)
            {
                output.WriteLine(bridge);
            }
        }
    }
}