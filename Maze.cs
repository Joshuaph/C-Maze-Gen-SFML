using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Markup;
using SFML.Graphics;
using SFML.Window;

namespace MazeGen
{
	public class Maze
	{
		public int NumberCells { get; set; }
		public int ImageSize { get; set; }
		public int TotalVisited { get; set; } = 0;
		public int CellSize => ImageSize / NumberCells;
		public RenderWindow MazeWindow;
		private int NumberColumns => ImageSize / CellSize;
		private int NumberRows => ImageSize / CellSize;
		private List<Cell> _grid;
		private List<Cell> _stack; //wtf is this
		private bool _redo = false;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="numberCells"></param>
		/// <param name="imageSize"></param>
		public Maze(int numberCells, int imageSize)
		{
			NumberCells = numberCells;
			ImageSize = imageSize;

			InitWindow();
			CreateMaze();
		}

		//Methods

		/// <summary>
		///     I think this does all the stuff 
		/// </summary>
		private void CreateMaze()
		{
			do
			{
				_redo = false;
				_grid.Clear();

				for (var y = 0; y < NumberRows; y++)
				for (var x = 0; x < NumberColumns; x++)
					_grid.Add(new Cell(x, y, CellSize));

				_redo = DisplayMaze();
			} while (_redo);
		}

		/// <summary>
		///  this should PROBABLY be in the CELL class but idk its 2 AM
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		private Cell CheckNeighbors(Cell cell)
		{
			//TODO:cry

			return new Cell();
		}

		/// <summary>
		///     Main 'game loop' of the Maze class
		/// </summary>
		/// <returns></returns>
		public bool DisplayMaze()
		{
			var current = _grid[0];

			while (MazeWindow.IsOpen)
			{
				MazeWindow.Clear(new SFML.Graphics.Color(69, 69, 69));
				Cell next;
				current.Current = true;

				//events
				MazeWindow.Closed += HandleWindowEvents(EventType.Closed);
				MazeWindow.KeyPressed += HandleWindowEvents(EventType.KeyPressed);
			}

			return false;
		}

		private EventHandler HandleWindowEvents(EventType e)
		{
			switch (e)
			{
				case EventType.Closed:
					return (sender, ev) => { ((Window) sender).Close(); };
				case EventType.KeyPressed:
					return (sender, ev) => { HandleKeys((KeyEventArgs) ev); };

				default:
					throw new ArgumentOutOfRangeException();
			}

			return null;
		}

		private void HandleKeys(KeyEventArgs e)
		{
			switch (e.Code)
			{
				case Keyboard.Key.Escape:
					_redo = false;
					MazeWindow.Close();
					break;
				case Keyboard.Key.Enter:
					_redo = true;
					break;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private int Index(int x, int y)
		{
			return 1;
		}

		private void InitWindow()
		{
			//Initialize Window 
			MazeWindow = new RenderWindow(new VideoMode((uint) ImageSize, (uint) ImageSize), "Maze",
				Styles.Titlebar | Styles.Close);
		}
	}
}