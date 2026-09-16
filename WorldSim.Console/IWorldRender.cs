using WorldSim.Core.World;

namespace WorldSim.Console.Rendering;

public interface IWorldRenderer
{
    void Render(World world);
}