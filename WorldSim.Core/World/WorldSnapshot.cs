using System;
using System.Collections.Generic;
using System.Text;

namespace WorldSim.Core.World
{
    public class WorldSnapshot
    {
        public int FormatVersion { get; set; } = 1;
        public int Whidth { get; set; }
        public int Height { get; set; }
        public int Seed { get; set; }
        public int Tick { get; set; }
        public CellsSnapshot[] Cells { get; set; } = Array.Empty<CellsSnapshot>();
    }
}
