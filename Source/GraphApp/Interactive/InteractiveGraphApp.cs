using GraphApp.Interactive;
using Sharprompt;
using System.Reflection;

namespace GraphApp
{
    internal class InteractiveGraphApp
    {
        private readonly List<ICommand> commands;
        private readonly CommandContext context;

        public InteractiveGraphApp()
        {
            // Load all the command types defined in the application.
            commands = new(
                from commandType in Assembly.GetExecutingAssembly().GetTypes()
                where commandType.GetInterfaces().Contains(typeof(ICommand))
                let command = Activator.CreateInstance(commandType) as ICommand
                where command != null
                select command);
            context = new();
        }

        internal async Task RunAsync()
        {
            // Handle the commands for the interactor.
            while (true)
            {
                string selection = Prompt.Select<string>(
                    "Select the option",
                    from x in commands
                    where context.CurrentGraphInstance != null || !x.RequiresGraph
                    select x.Name);
                var command = commands.First(x => x.Name == selection);
                if (command.RequiresGraph && context.CurrentGraphInstance == null)
                {
                    Console.WriteLine("The graph hasn't been loaded yet. Please, try load the graph before working.");
                    continue;
                }
                await command.ExecuteAsync(context);
            }
        }
    }
}