namespace MazeGen
{
	public class CellDto
	{
		public int Size { get; set; }
		public int BorderSize { get; set; }

		public CellDto(int size, int borderSize)
		{
			BorderSize = borderSize;
			Size = size;
		}
	}
}