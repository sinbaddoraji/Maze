using System;

namespace Maze
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Generating a solvable maze...");

                MazeGenerator mazeGenerator = new MazeGenerator(25);  // Create a maze of size 20x20
                mazeGenerator.GenerateSolvableMaze();
                mazeGenerator.SolveMaze();
                mazeGenerator.DisplayMaze();


                Console.WriteLine("\nPress any key to solve the maze");
                Console.ReadKey();
                mazeGenerator.DisplayPlayerJourney(200);

                Console.WriteLine("\nJourney complete! Press any key to start again");
                Console.ReadKey();
            }
        }
    }
}