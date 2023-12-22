using GraphApp.Graphs;

namespace GraphApp.Interactive
{
    /// <summary>
    /// Represents the data for the command execution.
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// The instance of the current loaded graph.
        /// </summary>
        public Graph? CurrentGraphInstance { get; set; }
    }
}