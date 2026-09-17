using System;
using System.Collections.Generic;
using System.Text;

namespace WorldSim.Core.World
{
    public class CellsSnapshot
    {
        public TerrainType Terrain { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int Resurces { get; set; }
        public int Elevation { get; set; }
    }
}
