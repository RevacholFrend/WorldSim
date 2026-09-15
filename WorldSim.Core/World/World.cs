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



        public void Initialize(int seed)
        {
            var random = new Random(seed);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var cell = new Cell();
                    int roll = random.Next(100);
                    cell.Terrain = roll switch
                    {
                        < 30 => TerrainType.Water,
                        < 60 => TerrainType.Land,
                        < 80 => TerrainType.Forest,
                        _ => TerrainType.Desert
                    };
                    cell.Temperature = random.Next(-10, 40);
                    cell.Humidity = random.Next(0, 100);
                    cell.Resources = random.Next(0, 100);
                    _cells[x, y] = cell;
                }
            }
        }

        public void Tick()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    ref var cell = ref _cells[x, y];
                    cell.Humidity = Math.Clamp(cell.Humidity + Random.Shared.Next(-2, 3), 0, 100);
                    cell.Temperature = Math.Clamp(cell.Temperature + Random.Shared.Next(-1, 2), -0,50);
                }
            }
        }
    }
}
    

