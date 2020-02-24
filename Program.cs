using System;
using SFML.Graphics;

namespace MazeGen
{
	class Program
	{
		private static void Main(string[] args)
		{
			var maze = new Maze(40, 800, 1);
			maze.WindowColor = new Color(69, 69, 69);
			maze.Run();
		}
	}
}