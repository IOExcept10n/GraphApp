namespace GraphApp.Interactive.Commands
{
    internal class ExitCommand : ICommand
    {
        public string Name => "Exit the application.";

        public Task ExecuteAsync(CommandContext context)
        {
            Console.WriteLine("The program is closing.");
            Environment.Exit(0);
            return Task.CompletedTask;
        }
    }
}