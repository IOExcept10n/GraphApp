namespace GraphApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // The program should work automatically
            if (args.Length > 0)
            {
                if (args.Length >= 2)
                {
                    AutoGraphSolver.SolveGraph(args[0], args[1]);
                }
                else
                {
                    AutoGraphSolver.SolveGraph(args[0], "output.txt");
                }
            }
            else if (File.Exists("input.txt"))
            {
                AutoGraphSolver.SolveGraph("input.txt", "output.txt");
            }
            else if (File.Exists("input.nel"))
            {
                AutoGraphSolver.SolveGraph("input.nel", "output.txt");
            }
            else
            {
                var interactor = new InteractiveGraphApp();
                await interactor.RunAsync();
            }
        }
    }
}