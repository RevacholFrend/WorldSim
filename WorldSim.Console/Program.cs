using System;
using System.Text;
using WorldSim.Console.Rendering;
using WorldSim.Core.World;

System.Console.OutputEncoding = System.Text.Encoding.UTF8;
System.Console.CursorVisible = false;

const int worldWidth = 130;
const int worldHeight = 30;
const int seed = 50064;

var world = new World(worldWidth, worldHeight,seed);
world.Generate();

IWorldRenderer render = new ConsoleRenderer();

const int ticksPerSecond = 2;
int tickDelayMs = 1000 / ticksPerSecond;

while(true)
{
    world.Tick();
    render.Render(world);
    Thread.Sleep(tickDelayMs);
}

namespace WorldSim.Core.World
{

   /*public class Program
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
    }*/
}