using System;
using System.Collections.Generic;

namespace WorldSim.Core.World
{
    public class World
    {
        public int Width { get; }
        public int Height { get; }
        public int Seed { get; }
        public int Tick { get; private set; }
        private readonly Cells[,] _cells;

        public World(int width, int height, int seed)
        {
            Width = width;
            Height = height;
            Seed = seed;
            Tick = 0;
            _cells = new Cells[width, height];
            
        }

        public ref Cells GetCell(int x, int y) => ref _cells[x, y];

        public void Generate()
        {
            var generator = new TerrainGenerator(Seed);
            generator.Generate(this);
        }



        /*public void Initialize(int seed)
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
        }*/

        public void Advance()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    ref var cell = ref _cells[x, y];
                    if (cell.Terrain == TerrainType.Water)
                    {
                        cell.Humidity = 100;
                    }
                    else
                    {
                        cell.Humidity = Math.Clamp(cell.Humidity + Random.Shared.Next(-2, 3), 0, 100);
                    }

                    cell.Temperature = Math.Clamp(cell.Temperature + Random.Shared.Next(-1, 2), -0, 50);
                }
            }
            Tick++;
            
        }

        public Dictionary<TerrainType, int> GetBiomeStats()
        {
            var stats = new Dictionary<TerrainType, int>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var biome = _cells[x, y].Terrain;
                    stats[biome] = stats.GetValueOrDefault(biome) + 1;
                }
            }
            return stats;
        }

        public WorldStats GetStats()
        {
            int water = 0, mountain = 0, tundra = 0, taiga = 0, grassland = 0, forest = 0, swamp = 0, desert = 0, saavanna = 0, tropicalRainforest = 0;
            long tempSum = 0, humiditySum = 0;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    ref var cell = ref _cells[x, y];
                    switch (cell.Terrain)
                    {
                        case TerrainType.Water: water++; break;
                        case TerrainType.Mountain: mountain++; break;
                        case TerrainType.Tundra: tundra++; break;
                        case TerrainType.Taiga: taiga++; break;
                        case TerrainType.Grassland: grassland++; break;
                        case TerrainType.Forest: forest++; break;
                        case TerrainType.Swamp: swamp++; break;
                        case TerrainType.Desert: desert++; break;
                        case TerrainType.Savanna: saavanna++; break;
                        case TerrainType.TropicalRainforest: tropicalRainforest++; break;
                    }
                    tempSum += cell.Temperature;
                    humiditySum += cell.Humidity;
                }
            

            int total = Width * Height;
            return new WorldStats(water, mountain, tundra, taiga, grassland, forest, swamp, desert, saavanna, tropicalRainforest,
                    (int)(tempSum/total),(int)(humiditySum/total));    

           
        }

        public WorldSnapshot ToSnapshot()
        {
            var snapshot = new WorldSnapshot
            {
                FormatVersion = 1,
                Whidth = Width,
                Height = Height,
                Seed = Seed,
                Tick = Tick,
                Cells = new CellsSnapshot[Width * Height]
            };
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    ref var cell = ref _cells[x,y];
                    snapshot.Cells[y * Width + x] = new CellsSnapshot
                    {
                        Terrain = cell.Terrain,
                        Elevation = cell.Elevation,
                        Humidity = cell.Humidity,
                        Temperature = cell.Temperature,
                        Resurces = cell.Resources
                    };
                }
            }
            return snapshot;
        }
        public static World FromSnapshot(WorldSnapshot snapshot)
        {
            if (snapshot.FormatVersion != 1)
                throw new InvalidDataException($"Неподдерживаемая версия: {snapshot.FormatVersion}");

            
            if (snapshot.Cells.Length != snapshot.Whidth * snapshot.Height)
                throw new InvalidDataException("Количество ячеек не совпадает");

            var world = new World(snapshot.Whidth, snapshot.Height,snapshot.Seed);
            world.Tick = snapshot.Tick;

            for (int y = 0;y < world.Height;y++)
            {
                for (int x = 0;x < world.Width;x++)
                {
                    var cellSnap = snapshot.Cells[y*world.Width + x];
                    ref var cell = ref world._cells[x,y];
                    cell.Terrain = cellSnap.Terrain;
                    cell.Elevation = cellSnap.Elevation;
                    cell.Humidity = cellSnap.Humidity;
                    cell.Temperature = cellSnap.Temperature;
                    cell.Resources = cellSnap.Resurces;
                }
            }

            return world;
        }

        public record WorldStats(int Water, int Mountain, int Tundra, int Taiga, int Grassland,
            int  Forest, int Swamp, int Desert, int Savanna, int TropicalRainforest, int AvgTemp, int AvgHumidity);

    }
}
    

