using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Markup;
using SFML.Graphics;
using SFML.Window;
using Color = SFML.Graphics.Color;

namespace MazeGen
{
	public class Maze
	{
/*
		 *  ----------------------------- VARIABLES ---------------------------
		 */

		//public
		public Color WindowColor { get; set; }


		//private
		private int _numberCells;
		private readonly int _imageSize;
		private int TotalVisited = 0;
		private readonly CellDto _cellData;
		private RenderWindow _mazeWindow;
		private int NumberColumns => _imageSize / _cellData.Size;
		private int NumberRows => _imageSize / _cellData.Size;
		private readonly List<Cell> _grid = new List<Cell>();
		private List<Cell> _stack; //wtf is this
		private bool _redo;


		/*
		 *  ----------------------------- CONSTRUCTOR ---------------------------
		 */


		/// <summary>
		/// Initializes Maze
		/// </summary>
		/// <param name="numberCells">X*X size grid where x = numberCells</param>
		/// <param name="imageSize">Length of sides for the squared window</param>
		/// <param name="borderSize">Size of the border of each cell</param>
		public Maze(int numberCells, int imageSize, int borderSize)
		{
			_numberCells = numberCells;
			_imageSize = imageSize;
			_cellData = new CellDto(imageSize / numberCells, borderSize);
			Init();
		}


		/*
		 *  ----------------------------- METHODS ---------------------------
		 */


		/// <summary>
		/// Main Run functions of this class.
		/// 	Sets up the grid list and calls the display function
		/// </summary>
		public void Run()
		{
			do
			{
				_redo = false;
				_grid.Clear();

				//sets up the grid(list) of cells Number Cells / Number Cells grid
				for (var y = 0; y < NumberRows; y++)
				for (var x = 0; x < NumberColumns; x++)
					_grid.Add(new Cell(x, y, _cellData));
				_redo = DisplayMaze();
			} while (_redo);
		}


		private void DumbMazeCrap()
		{
			var neighbors = new List<Cell>();
			
			
		}

		/// <summary>
		/// Main 'game loop' of the Maze class
		/// 	Handles some event handling, and the main window
		/// </summary>
		/// <returns>a bool letting the program know if it should redo the whole grid</returns>
		private bool DisplayMaze()
		{
			var current = _grid[0];

			//events
			_mazeWindow.Closed += (s, e) => (s as Window)?.Close();
			_mazeWindow.KeyPressed += HandleKeys;

			while (_mazeWindow.IsOpen)
			{
				_mazeWindow.Clear(WindowColor);
				_mazeWindow.DispatchEvents();

				//render each cell
				foreach (var c in _grid)
					c.RenderCell(_mazeWindow);
				_mazeWindow.Display();
			}

			return false;
		}

		/// <summary>
		/// 
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
					case Keyboard.Key.Enter:
						_redo = true;
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
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private int Index(int x, int y)
		{
			return 1;
		}

		private void Init()
		{
			//Initialize Window 
			_mazeWindow = new RenderWindow(new VideoMode((uint) _imageSize, (uint) _imageSize), "Maze",
				Styles.Titlebar | Styles.Close);
		}


		private void GetCellIndex(int x, int y)
		{
			
		}
	}
}