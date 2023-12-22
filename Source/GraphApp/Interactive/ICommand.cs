namespace GraphApp.Interactive
{
    /// <summary>
    /// Represents an interface for the interactive commands.
    /// </summary>
    public interface ICommand : IIdentifiable
    {
        /// <summary>
        /// Gets the value that determines whether the command requires the graph for work.
        /// If the graph isn't loaded, the command won't be available for selection.
        /// </summary>
        public bool RequiresGraph => true;

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="context">The context for a command.</param>
        public Task ExecuteAsync(CommandContext context);
    }
}