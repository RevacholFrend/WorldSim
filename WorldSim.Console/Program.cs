using System.Text.Json;
using System.Text.Json.Serialization;
using WorldSim.Console.Rendering;
using WorldSim.Core.World;

System.Console.OutputEncoding = System.Text.Encoding.UTF8;
System.Console.CursorVisible = false;

const int worldWidth = 130;
const int worldHeight = 27;
const int seed = 12;

var world = new World(worldWidth, worldHeight,seed);
world.Generate();

var snapshot = world.ToSnapshot();
var  json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
{
    WriteIndented = true,
    Converters = {new JsonStringEnumConverter() }
});
File.WriteAllText("world.json", json);

var loadJson = File.ReadAllText("world.json");
var loadSnapshot = JsonSerializer.Deserialize<WorldSnapshot>(loadJson, new JsonSerializerOptions
{
    Converters = {new JsonStringEnumConverter() }
});
if (loadSnapshot is null)
    throw new InvalidDataException("Не удалось загрузить снапшот.");

var restoreWorld = World.FromSnapshot(loadSnapshot);

IWorldRenderer render = new ConsoleRenderer();

const int ticksPerSecond = 2;
int tickDelayMs = 1000 / ticksPerSecond;

while(true)
{
    var stats = world.GetStats();
    world.Advance();
    render.Render(world);
    Thread.Sleep(tickDelayMs);
}

