using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

// ReSharper disable InvertIf

namespace MazeGen
{
	/// <summary>
	/// 	enum to hold direction yay!
	/// </summary>
	public enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}

	public class Maze
	{
		/*
		 *  ----------------------------- Variables ---------------------------
		 */

		//private
		private readonly int _numberCells;
		private readonly List<Cell> _grid = new List<Cell>();
		private readonly int _imageSize;
		private readonly int _cellSize;
		private readonly int _borderSize;
		private RenderWindow _mazeWindow;
		private bool _redo;
		private int NumberColumns => _imageSize / _cellSize;
		private int NumberRows => NumberColumns;
		public Color WindowColor { get; set; }
		public bool DisplayWhileDrawing { get; set; }

		/*
		 *  ----------------------------- CONSTRUCTOR ---------------------------
		 */


		/// <summary>
		///     Maze Constructor
		/// </summary>
		/// <param name="numberCells">X*X size grid where x = numberCells</param>
		/// <param name="imageSize">Length of sides for the squared window</param>
		/// <param name="borderSize">Size of the border of each cell</param>
		public Maze(int numberCells, int imageSize, int borderSize)
		{
			_numberCells = (int) Math.Pow(numberCells, 2);
			_imageSize = imageSize;
			_cellSize = imageSize / numberCells;
			_borderSize = borderSize;
			_redo = false;
		}


		/*
		 *  ----------------------------- METHODS ---------------------------
		 */


		/// <summary>
		///     Main Run functions of this class.
		///     Sets up the grid list and calls the display function
		/// </summary>
		public void Run()
		{
			Init();
			DisplayGrid();
		}


		/// <summary>
		///     Main 'game loop' of the Maze class
		///     Handles some event handling, and the main window
		/// </summary>
		/// <returns>a bool letting the program know if it should redo the whole grid</returns>
		private void DisplayGrid()
		{
			var done = false;

			//events
			_mazeWindow.Closed += (s, e) => (s as Window)?.Close();
			_mazeWindow.KeyPressed += HandleKeys;

			var rand = new Random(Guid.NewGuid().GetHashCode());

			while (_mazeWindow.IsOpen)
			{
				_mazeWindow.Clear(WindowColor);
				_mazeWindow.DispatchEvents();

				//render each cell
				foreach (var c in _grid)
					c.RenderCell(_mazeWindow);

				if (!done || _redo)
				{
					done = false;
					_redo = false;
					RecursiveBacktracer(_grid[rand.Next(_numberCells)], rand);
				}

				if (!_redo)
					done = true;

				_mazeWindow.Display();
			}
		}

		/// <summary>
		/// 	Handles all key presses/input
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private void HandleKeys(object? sender, KeyEventArgs e)
		{
			try
			{
				switch (e.Code)
				{
					case Keyboard.Key.Escape:
						_redo = false;
						_mazeWindow.Close();
						break;
					case Keyboard.Key.Enter: //restart the maze
						_redo = true;
						Init();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Console.WriteLine($"Key {ex} not supported.");
			}
		}

		/// <summary>
		/// 	Sets up the grid with fresh cells,
		/// 		and if the main window hasn't been created it will create one
		/// </summary>
		private void Init()
		{
			//sets up the grid(list) of cells Number Cells / Number Cells grid
			_grid.Clear();
			for (var y = 0; y < NumberRows; y++)
			for (var x = 0; x < NumberColumns; x++)
				_grid.Add(new Cell(x, y, _cellSize, _borderSize, WindowColor));

			//Initialize Window 
			if (_mazeWindow is null)
				_mazeWindow = new RenderWindow(new VideoMode((uint) _imageSize, (uint) _imageSize), "Maze",
					Styles.Titlebar | Styles.Close);
		}


		/// <summary>
		/// 	Recursive method that carves out the Maze.
		/// 	If currentCell has valid neighbors, randomly choose a neighbor to visit recursively.
		/// </summary>
		/// <param name="currentCell"></param>
		/// <param name="rand"></param>
		private void RecursiveBacktracer(Cell currentCell, Random rand)
		{
			//loop to visit ALL unvisited Neighbors.
			//This basically overtakes the main game loop when running
			do
			{
				//clear and dispatch events for window
				_mazeWindow.DispatchEvents();

				//update current cell
				currentCell.Visited = true;
				currentCell.Current = true;


				//render all cells
				if (DisplayWhileDrawing)
				{
					_mazeWindow.Clear(WindowColor);
					foreach (var c in _grid)
						c.RenderCell(_mazeWindow);
					_mazeWindow.Display();
				}


				//If we have valid neighbors pick one and visit them
				// ReSharper disable once InvertIf
				if (GetNeighbor(currentCell).Item2 != -1 && !_redo)
				{
					//get random neighbor
					var (dir, i) = GetNeighbor(currentCell);
					var next = _grid[i];

					//remove walls
					currentCell.RemoveWalls((int) dir);
					next.RemoveOpposite((int) dir);

					//update current
					currentCell.Current = false;

					//go to next cell
					RecursiveBacktracer(next, rand);
				}
			} while (GetNeighbor(currentCell).Item2 != -1 && !_redo);

			//update current cell
			currentCell.Current = false;
		}


		/// <summary>
		/// 	Grabs all valid neighbors.
		/// 		Neighbors are valid if they exist, and haven't been visited
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		private Tuple<Direction, int> GetNeighbor(Cell cell)
		{
			var neighbors = new List<Tuple<Direction, int>>();
			var rand = new Random(Guid.NewGuid().GetHashCode());

			//get current location of cell
			var c = cell.ColumnNumber;
			var r = cell.RowNumber;
			var i = NumberColumns * r + c; //current cell index

			//Checks the bounds of the neighbor and if it was visited. 
			//the random -1 || +1 is because of things being 0 indexed
			if (c != 0)
				if (!_grid[i - 1].Visited)
					neighbors.Add(new Tuple<Direction, int>(Direction.Left, i - 1));
			if (cell.ColumnNumber != (NumberColumns - 1))
				if (!_grid[i + 1].Visited)
					neighbors.Add(new Tuple<Direction, int>(Direction.Right, i + 1));
			if (cell.RowNumber != 0)
				if (!_grid[i - (NumberColumns)].Visited)
					neighbors.Add(new Tuple<Direction, int>(Direction.Up, i - NumberColumns));
			if (cell.RowNumber != (NumberRows - 1))
				if (!_grid[i + (NumberColumns)].Visited)
					neighbors.Add(new Tuple<Direction, int>(Direction.Down, i + NumberColumns));

			return neighbors.Count != 0
				? neighbors[rand.Next(neighbors.Count)]
				: new Tuple<Direction, int>(Direction.Down, -1);
		}

		/// <summary>
		/// 	Neighbor class that holds direction and the cell.
		/// 		Created this so I can put it in a list.
		/// </summary>
		private class Neighbor
		{
			/// <summary>
			/// 	Direction relative to our current cell
			/// </summary>
			public Direction Direction { get; }

			/// <summary>
			/// 	Our neighbor cell
			/// </summary>
			public Cell Cell { get; }

			/// <summary>
			/// 	Neighbor Constructor
			/// </summary>
			/// <param name="direction"></param>
			/// <param name="cell"></param>
			public Neighbor(Direction direction, Cell cell)
			{
				Cell = cell;
				Direction = direction;
			}
		}
	}
}