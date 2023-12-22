namespace GraphApp.Interactive
{
    /// <summary>
    /// Represents an interface for the interactive commands.
    /// </summary>
    public interface ICommand : IIdentifiable
    {
        public bool RequiresGraph => true;

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="context">The context for a command.</param>
        public Task ExecuteAsync(CommandContext context);
    }
}