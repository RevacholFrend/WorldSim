using System.Text;
using WorldSim.Core.World;

namespace WorldSim.Console.Rendering;

public class ConsoleRenderer : IWorldRenderer
{
    // Буфер кадра: список сегментов "цвет + текст".
    // Переиспользуем между кадрами, чтобы не аллоцировать каждый раз.
    private readonly List<(ConsoleColor color, string text)> _frameBuffer = new();

    // Временный StringBuilder для накопления символов одного цвета.
    private readonly StringBuilder _segmentBuilder = new();

    public void Render(World world)
    {
        // 1. Собираем весь кадр в буфер (в памяти, без вывода в консоль).
        BuildFrame(world);

        // 2. Выводим буфер одним проходом.
        FlushFrame();
    }

    /// <summary>
    /// Собирает кадр в _frameBuffer. Ничего не пишет в консоль.
    /// </summary>
    private void BuildFrame(World world)
    {
        _frameBuffer.Clear();

        for (int y = 0; y < world.Height; y++)
        {
            ConsoleColor currentColor = ConsoleColor.Gray;
            _segmentBuilder.Clear();

            for (int x = 0; x < world.Width; x++)
            {
                var cell = world.GetCell(x, y);
                var (symbol, color) = GetVisual(cell.Terrain);

                // Если цвет сменился — закрываем текущий сегмент и начинаем новый.
                if (color != currentColor && _segmentBuilder.Length > 0)
                {
                    _frameBuffer.Add((currentColor, _segmentBuilder.ToString()));
                    _segmentBuilder.Clear();
                }

                currentColor = color;
                _segmentBuilder.Append(symbol);
            }

            // Закрываем последний сегмент строки.
            if (_segmentBuilder.Length > 0)
            {
                _frameBuffer.Add((currentColor, _segmentBuilder.ToString()));
                _segmentBuilder.Clear();
            }

            // Перенос строки — как отдельный серый сегмент.
            _frameBuffer.Add((ConsoleColor.Gray, Environment.NewLine));
        }

        // Легенда и статистика — тоже в буфер.
        AppendLegend(world);
    }

    /// <summary>
    /// Выводит собранный буфер в консоль. Один проход, без Console.Clear().
    /// </summary>
    private void FlushFrame()
    {
        // Ставим курсор в начало — перерисовываем поверх старого кадра.
        System.Console.SetCursorPosition(0, 0);

        foreach (var (color, text) in _frameBuffer)
        {
            System.Console.ForegroundColor = color;
            System.Console.Write(text);
        }

        System.Console.ResetColor();
    }

    private void AppendLegend(World world)
    {
        _frameBuffer.Add((ConsoleColor.Gray, Environment.NewLine));
        _frameBuffer.Add((ConsoleColor.Gray, "~ Water | ^ Mountain | . Tundra | t Taiga | , Grassland"));
        _frameBuffer.Add((ConsoleColor.Gray, Environment.NewLine));
        _frameBuffer.Add((ConsoleColor.Gray, "# Forest | % Swamp | : Desert | ; Savanna | @ Rainforest"));
        _frameBuffer.Add((ConsoleColor.Gray, Environment.NewLine));
        _frameBuffer.Add((ConsoleColor.Gray, Environment.NewLine));

        var stats = world.GetBiomeStats();
        int total = world.Width * world.Height;

        foreach (var (biome, count) in stats.OrderByDescending(kv => kv.Value))
        {
            int percent = count * 100 / total;
            string line = $"{biome,-20} {percent,3}%{Environment.NewLine}";
            _frameBuffer.Add((ConsoleColor.Gray, line));
        }
    }

    private static (char symbol, ConsoleColor color) GetVisual(TerrainType terrain) => terrain switch
    {
        TerrainType.Water => ('~', ConsoleColor.Blue),
        TerrainType.Mountain => ('^', ConsoleColor.Gray),
        TerrainType.Tundra => ('.', ConsoleColor.White),
        TerrainType.Taiga => ('t', ConsoleColor.DarkGreen),
        TerrainType.Grassland => (',', ConsoleColor.Green),
        TerrainType.Forest => ('#', ConsoleColor.DarkGreen),
        TerrainType.Swamp => ('%', ConsoleColor.DarkYellow),
        TerrainType.Desert => (':', ConsoleColor.Yellow),
        TerrainType.Savanna => (';', ConsoleColor.DarkYellow),
        TerrainType.TropicalRainforest => ('@', ConsoleColor.Green),
        _ => ('?', ConsoleColor.Magenta)
    };
}