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
        public Vertex[] LineArr { get; set; }

        public Wall(Vertex[] lineArr)
        {
            IsVisible = true;
            LineArr = lineArr;
        }
    }


    public class Cell
    {
        //colors
        Color _linesColor = Color.White;
        Color _visitedColor = Color.Magenta;
        Color _currentColor = Color.Green;

        //vars
        private int XStart => ColumnNumber == 0 ? 0 : ColumnNumber * Size;
        private int XEnd => ColumnNumber == 0 ? Size : Size * ColumnNumber;
        private int YStart => RowNumber == 0 ? 0 : RowNumber * Size;
        private int YEnd => RowNumber == 0 ? Size : Size * RowNumber;
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public int Size { get; set; }

        public bool Visited { get; set; }
        public bool Current { get; set; }

        private Dictionary<string, Wall> borderDict = new Dictionary<string, Wall>();
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
            InitCell();
        }

        private void InitCell()
        {
            //create cell shape
            _cellShape = new RectangleShape(new Vector2f(Size, Size)) {
                FillColor = _visitedColor,
                Position = new Vector2f(XStart, YStart)
            };

            //add borders to dictionary
            borderDict.Add("topSide", new Wall(new[] {
                new Vertex(new Vector2f(XStart, YStart), _linesColor),
                new Vertex(new Vector2f(XEnd, YStart), _linesColor)
            }));

            borderDict.Add("rightSide", new Wall(new[] {
                new Vertex(new Vector2f(XEnd, YStart), _linesColor),
                new Vertex(new Vector2f(XEnd, YEnd), _linesColor)
            }));
            borderDict.Add("bottomSide", new Wall(new[] {
                new Vertex(new Vector2f(XStart, YEnd), _linesColor),
                new Vertex(new Vector2f(XEnd, YEnd), _linesColor)
            }));
            borderDict.Add("leftSide", new Wall(new[] {
                new Vertex(new Vector2f(XStart, YStart), _linesColor),
                new Vertex(new Vector2f(XStart, YEnd), _linesColor)
            }));
        }

        /// <summary>
        ///     Method to render cell to window 
        /// </summary>
        /// <param name="window"></param>
        public void RenderCell(RenderWindow window)
        {
            //change color
            if (Current || Visited && Current)
                _cellShape.FillColor = _currentColor;
            else if (Visited)
                _cellShape.FillColor = _visitedColor;

            //display walls
            foreach (var obj in borderDict.Where(line => line.Value.IsVisible))
                window.Draw(obj.Value.LineArr, PrimitiveType.Lines);
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
                borderDict[val].IsVisible = false;
        }


        //stupid fix to decode an array
        private List<String> DecodeWalls(IEnumerable<int> x)
        {
            var decodedList = new List<string>();

            foreach (var val in x)
                switch (val)
                {
                    case 1:
                        decodedList.Add("topSide");
                        break;
                    case 2:
                        decodedList.Add("rightSide");
                        break;
                    case 3:
                        decodedList.Add("bottomSide");
                        break;
                    case 4:
                        decodedList.Add("leftSide");
                        break;
                }

            return decodedList;
        }
    }
}