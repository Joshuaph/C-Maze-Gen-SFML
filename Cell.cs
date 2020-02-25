using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SFML.Graphics;
using SFML.System;

namespace MazeGen
{
	public class Cell
	{
		/*
		 *  ----------------------------- Variables ---------------------------
		 */

		//private
		private readonly Dictionary<string, Wall> _borderDict = new Dictionary<string, Wall>();
		private readonly Color _linesColor = Color.White;
		private readonly Color _currentColor = Color.Green;
		private readonly Color _baseColor;
		private readonly int _lineWidth;
		private RectangleShape _cellShape;
		private Vector2f _position;

		//public
		public int RowNumber { get; }
		public int ColumnNumber { get; }
		private int Size { get; }
		public bool Visited { get; set; }
		public bool Current { get; set; }
		/*
		 *  ----------------------------- Constructor ---------------------------
		 */


		/// <summary>
		/// Constructor 
		///     Initialize new cell with border size
		/// </summary>
		/// <param name="columnNumber">cell column number (0 indexed)</param>
		/// <param name="rowNumber">cell row number (0 indexed)</param>
		/// <param name="size">cell size</param>
		/// <param name="lineWidth">border width</param>
		/// <param name="cor">cell color</param>
		public Cell(int columnNumber, int rowNumber, int size, int lineWidth, Color cor)
		{
			ColumnNumber = columnNumber;
			RowNumber = rowNumber;
			Size = size;
			_lineWidth = lineWidth;
			_baseColor = cor;
			InitCell();
		}


		/*
		 *  ----------------------------- Methods ---------------------------
		 */

		/// <summary>
		/// 	Initializes the position, shape, and borders of the cell.
		/// 		
		/// </summary>
		private void InitCell()
		{
			_position = new Vector2f(ColumnNumber * Size, RowNumber * Size);
			//create cell shape
			_cellShape = new RectangleShape(new Vector2f(Size, Size))
			{
				FillColor = _baseColor,
				Position = _position
			};

			//add borders to dictionary
			_borderDict.Add("topSide",
				new Wall(new Vector2f(Size, _lineWidth), new Vector2f(_position.X, _position.Y), _linesColor));
			_borderDict.Add("rightSide",
				new Wall(new Vector2f(_lineWidth, Size), new Vector2f(_position.X + Size - _lineWidth, _position.Y),
					_linesColor));
			_borderDict.Add("bottomSide",
				new Wall(new Vector2f(Size, _lineWidth), new Vector2f(_position.X, _position.Y + Size - _lineWidth),
					_linesColor));
			_borderDict.Add("leftSide",
				new Wall(new Vector2f(_lineWidth, Size), new Vector2f(_position.X, _position.Y), _linesColor));
		}

		/// <summary>
		///     Method to render the cell to window 
		/// </summary>
		/// <param name="window"></param>
		public void RenderCell(RenderWindow window)
		{
			//change color
			_cellShape.FillColor = Current ? _currentColor : _baseColor;

			window.Draw(_cellShape);
			//display walls
			foreach (var obj in _borderDict.Where(line => line.Value.IsVisible))
				window.Draw(obj.Value.RecWall);
		}

		/// <summary>
		/// 	takes an array of wall sides that can be removed
		/// </summary>
		/// <param name="arr">array of walls to be removed</param>
		public void RemoveWalls(IEnumerable<int> arr)
		{
			RemoveWalls(DecodeWalls(arr));
		}

		/// <summary>
		/// 	takes an int describing which wall number to remove
		/// </summary>
		/// <param name="x">wall number to be removed</param>
		public void RemoveWalls(int x)
		{
			//didnt want to make another DecodeWalls method so I 'cast' var x into an array LMAO

			RemoveWalls(DecodeWalls(new int[] {x}));
		}

		/// <summary>
		///    Actually removes the walls
		/// </summary>
		/// <param name="arr"></param>
		public void RemoveWalls(IEnumerable<string> arr)
		{
			foreach (var val in arr)
				_borderDict[val].IsVisible = false;
		}


		/// <summary>
		///     Decodes ints into wall sides 
		/// </summary>
		/// <param name="x"></param>
		private static IEnumerable<string> DecodeWalls(IEnumerable<int> x)
		{
			var decodedList = new List<string>();

			foreach (var val in x)
				switch (val)
				{
					case 0:
						decodedList.Add("topSide");
						break;
					case 1:
						decodedList.Add("rightSide");
						break;
					case 2:
						decodedList.Add("bottomSide");
						break;
					case 3:
						decodedList.Add("leftSide");
						break;
				}

			return decodedList;
		}

		/// <summary>
		///     Removes the opposite wall
		/// </summary>
		/// <param name="wall"></param>
		public void RemoveOpposite(int wall)
		{
			switch (wall)
			{
				case 0:
					RemoveWalls(2);
					break;
				case 1:
					RemoveWalls(3);
					break;
				case 2:
					RemoveWalls(0);
					break;
				case 3:
					RemoveWalls(1);
					break;
			}
		}

		/// <summary>
		/// 	Wall class holds rectangle of wall and visible bool
		/// </summary>
		private class Wall
		{
			public bool IsVisible { get; set; }
			public RectangleShape RecWall { get; set; }

			public Wall(Vector2f size, Vector2f position, Color cor)
			{
				IsVisible = true;
				RecWall = new RectangleShape(size) {FillColor = cor, Position = position};
			}
		}
	}
}