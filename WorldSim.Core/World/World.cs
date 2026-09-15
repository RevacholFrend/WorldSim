using System;
using System.Collections.Generic;
using System.Text;

namespace WorldSim.Core.World
{
    public class World
    {
        public int Width { get; }
        public int Height { get; }
        private readonly Cell[,] _cells;

        public World(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new Cell[width, height];
        }

        public ref Cell GetCell(int x, int y) => ref _cells[x, y];
    }
}
