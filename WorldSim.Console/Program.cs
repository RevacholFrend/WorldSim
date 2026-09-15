using System;
using System.Text;

namespace WorldSim.Core.World
{
   public class Program
    {
        static void Main(string[] args)
        {
            int seed = 5552465;
            World world = new World(29, 50);
            world.Initialize(seed);
            while (true)
            {
                world.Tick();
                Console.Clear();
                RenderWorld(world);
                Thread.Sleep(500);
            }
            Console.ReadLine();
        }
        static void RenderWorld(World world)
        {
            for (int i = 0; i < world.Width; i++)
            {
                for (int j = 0; j < world.Height; j++)
                {
                    var cell = world.GetCell(i, j);
                    char c = cell.Terrain switch
                    {
                        TerrainType.Water => '~',
                        TerrainType.Land => ',',
                        TerrainType.Forest => '#',
                        TerrainType.Desert => ':',
                        _=> '?'
                    };
                    Console.Write(c);

                }
                Console.WriteLine();
            }

        }
    }
}