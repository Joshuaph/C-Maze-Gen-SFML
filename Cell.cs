using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SFML.Graphics;
using SFML.System;

namespace MazeGen
{
	public class Wall
	{
		public bool IsVisible { get; set; }
		public RectangleShape RecWall { get; set; }


		public Wall(Vector2f size, Vector2f position, Color cor)
		{
			IsVisible = true;
			RecWall = new RectangleShape(size) {FillColor = cor, Position = position};
		}
	}


	public class Cell
	{
		//colors
		private readonly Color _linesColor = Color.White;
		private readonly Color _visitedColor = Color.Magenta;
		private readonly Color _currentColor = Color.Green;
		private readonly Color _baseColor = new Color(69,69,69);
		private readonly int _lineWidth = 2;

		//vars
		public Vector2f Position;
		public int RowNumber { get; set; }
		public int ColumnNumber { get; set; }
		public int Size { get; set; }

		public bool Visited { get; set; }
		public bool Current { get; set; }

		private readonly Dictionary<string, Wall> _borderDict = new Dictionary<string, Wall>();
		private RectangleShape _cellShape;

		//constructors
		public Cell()
		{
			ColumnNumber = 0;
			RowNumber = 0;
			Size = 10;
			InitCell();
		}

		/// <summary>
		///     Initialize new cell
		/// </summary>
		/// <param name="columnNumber">Column number of this cell</param>
		/// <param name="rowNumber">Row number of this cell</param>
		/// <param name="size">size of cell</param>
		public Cell(int columnNumber, int rowNumber, int size)
		{
			ColumnNumber = columnNumber;
			RowNumber = rowNumber;
			Size = size;
			Position = new Vector2f(columnNumber * size, rowNumber * size);
			InitCell();
		}

		/// <summary>
		///     Initialize new cell with bordersize
		/// </summary>
		/// <param name="columnNumber"></param>
		/// <param name="rowNumber"></param>
		/// <param name="cellDto"></param>
		public Cell(int columnNumber, int rowNumber, CellDto cellDto)
		{
			ColumnNumber = columnNumber;
			RowNumber = rowNumber;
			Size = cellDto.Size;
			_lineWidth = cellDto.BorderSize;
			InitCell();
		}

		private void InitCell()
		{
			Position = new Vector2f(ColumnNumber * Size, RowNumber * Size);
			//create cell shape
			_cellShape = new RectangleShape(new Vector2f(Size, Size))
			{
				FillColor = _baseColor,
				Position = Position
			};

			//add borders to dictionary
			_borderDict.Add("topSide",
				new Wall(new Vector2f(Size, _lineWidth), new Vector2f(Position.X, Position.Y), _linesColor));
			_borderDict.Add("rightSide",
				new Wall(new Vector2f(_lineWidth, Size), new Vector2f(Position.X + Size - _lineWidth, Position.Y),
					_linesColor));
			_borderDict.Add("bottomSide",
				new Wall(new Vector2f(Size, _lineWidth), new Vector2f(Position.X, Position.Y + Size - _lineWidth),
					_linesColor));
			_borderDict.Add("leftSide",
				new Wall(new Vector2f(_lineWidth, Size), new Vector2f(Position.X, Position.Y), _linesColor));
		}

		/// <summary>
		///     Method to render cell to window 
		/// </summary>
		/// <param name="window"></param>
		public void RenderCell(RenderWindow window)
		{
			//change color
			if (Current)
			    _cellShape.FillColor = _currentColor;
			else
				_cellShape.FillColor = _baseColor;
			
			window.Draw(_cellShape);
			//display walls
			foreach (var obj in _borderDict.Where(line => line.Value.IsVisible))
				window.Draw(obj.Value.RecWall);
		}

		//array of walls to remove
		public void RemoveWalls(IEnumerable<int> arr)
		{
			RemoveWalls(DecodeWalls(arr));
		}

		//int of wall to remove (1-4)
		public void RemoveWalls(int x)
		{
			//didnt want to make another DecodeWalls method so I 'cast' var x into an array LMAO
			RemoveWalls(DecodeWalls(new int[] {x}));
		}

		public void RemoveWalls(IEnumerable<string> arr)
		{
			foreach (var val in arr)
				_borderDict[val].IsVisible = false;
		}


		//stupid fix to decode an array
		private static List<String> DecodeWalls(IEnumerable<int> x)
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

		public void RemoveOpposite(int wall)
		{
			switch (wall)
			{
				case 0: RemoveWalls(2);
					break;
				case 1:RemoveWalls(3);
					break;
				case 2:RemoveWalls(0);
					break;
				case 3: RemoveWalls(1);
					break;
			}
		}
	}
}