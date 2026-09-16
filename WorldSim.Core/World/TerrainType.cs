using System;
using System.Collections.Generic;
using System.Text;

namespace WorldSim.Core.World
{
    public enum TerrainType
    {
        Water,                // океаны, моря, озёра
        Mountain,             // высокогорье
        Tundra,               // холодно + сухо
        Taiga,                // холодно + влажно (хвойные леса)
        Grassland,            // умеренно + сухо (степи, луга)
        Forest,               // умеренно + средне
        Swamp,                // умеренно + очень влажно
        Desert,               // жарко + сухо
        Savanna,              // жарко + средне
        TropicalRainforest    // жарко + влажно

    }
}
