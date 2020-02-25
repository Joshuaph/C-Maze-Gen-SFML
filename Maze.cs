using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace MazeGen
{
    
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    public class Maze
    {
        private readonly CellDto _cellData;
        private readonly List<Cell> _grid = new List<Cell>();
        private readonly int _imageSize;


        //private
        private readonly int _numberCells;
        private RenderWindow _mazeWindow;
        private bool _redo;


        /*
         *  ----------------------------- CONSTRUCTOR ---------------------------
         */


        /// <summary>
        ///     Initializes Maze
        /// </summary>
        /// <param name="numberCells">X*X size grid where x = numberCells</param>
        /// <param name="imageSize">Length of sides for the squared window</param>
        /// <param name="borderSize">Size of the border of each cell</param>
        public Maze(int numberCells, int imageSize, int borderSize)
        {
            _numberCells = (int) Math.Pow(numberCells, 2);
            _imageSize = imageSize;
            _cellData = new CellDto(imageSize / numberCells, borderSize);
        }
/*
		 *  ----------------------------- VARIABLES ---------------------------
		 */

        //public
        public Color WindowColor { get; set; }
        private int NumberColumns => _imageSize / _cellData.Size;
        private int NumberRows => NumberColumns;


        /*
         *  ----------------------------- METHODS ---------------------------
         */


        /// <summary>
        ///     Main Run functions of this class.
        ///     Sets up the grid list and calls the display function
        /// </summary>
        public void Run()
        {
            _redo = false;
            Init();
            DisplayGrid();
        }


        /// <summary>
        ///     Main 'game loop' of the Maze class
        ///     Handles some event handling, and the main window
        /// </summary>
        /// <returns>a bool letting the program know if it should redo the whole grid</returns>
        private bool DisplayGrid()
        {
            var done = false;
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

                //random start point
                var rand = new Random(Guid.NewGuid().GetHashCode());
                if(!done || _redo)
                    RecursiveBacktracer(_grid[rand.Next(_numberCells)]);
                
                if(!_redo)
                    done = true;

                _mazeWindow.Display();
            }

            return false;
        }

        /// <summary>
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

        private void Init()
        {
            _grid.Clear();
            //sets up the grid(list) of cells Number Cells / Number Cells grid
            for (var y = 0; y < NumberRows; y++)
            for (var x = 0; x < NumberColumns; x++)
                _grid.Add(new Cell(x, y, _cellData));
            
            if(_mazeWindow is null)
                //Initialize Window 
                _mazeWindow = new RenderWindow(new VideoMode((uint) _imageSize, (uint) _imageSize), "Maze",
                    Styles.Titlebar | Styles.Close);
        }


        private void RecursiveBacktracer(Cell currentCell)
        {
            _redo = false;
            var rand = new Random(Guid.NewGuid().GetHashCode());
            
            var neighbors = new List<Neighbor>();

            do
            {
                currentCell.Visited = true;
                currentCell.Current = true;

                _mazeWindow.Clear(WindowColor);
                _mazeWindow.DispatchEvents();

                foreach (var c in _grid)
                    c.RenderCell(_mazeWindow);
                _mazeWindow.Display();

                neighbors.Clear();
                neighbors = GetNeighbors(currentCell);
                if (neighbors.Count != 0 && !_redo)
                {
                    var ran = rand.Next(neighbors.Count);
                    var neighbor = neighbors[ran];
                    neighbors.RemoveAt(ran);
                    var next = neighbor.Cell;
                    var dir = neighbor.Direction;

                    currentCell.RemoveWalls((int) dir);
                    next.RemoveOpposite((int) dir);
                    currentCell.Current = false;
                    RecursiveBacktracer(next);
                }
            } while (neighbors.Count > 0 && !_redo);

            currentCell.Current = false;
        }

        private List<Neighbor> GetNeighbors(Cell cell)
        {
            var neighbors = new List<Neighbor>();
            var c = cell.ColumnNumber;
            var r = cell.RowNumber;
            var i = NumberColumns * r + c;
            if (c != 0)
                if (!_grid[i - 1].Visited)
                    neighbors.Add(new Neighbor(Direction.Left, _grid[i - 1]));
            if (cell.ColumnNumber != (NumberColumns-1))
                if (!_grid[i + 1].Visited)
                    neighbors.Add(new Neighbor(Direction.Right, _grid[i + 1]));
            if (cell.RowNumber != 0)
                if (!_grid[i - (NumberColumns)].Visited)
                    neighbors.Add(new Neighbor(Direction.Up, _grid[i - NumberColumns]));
            if (cell.RowNumber != (NumberRows-1))
                if (!_grid[i + (NumberColumns)].Visited)
                    neighbors.Add(new Neighbor(Direction.Down, _grid[i + NumberColumns]));
            
            return neighbors;
        }
        //private List<Cell> _stack = new List<Cell>();


        private class Neighbor
        {
            public Direction Direction { get; set; }
            public Cell Cell { get; set; }

            public Neighbor(Direction direction, Cell cell)
            {
                Cell = cell;
                Direction = direction;
            }
        }
    }
}