using System;


namespace WorldSim.Core.World;

/// <summary>
/// Генератор мира на основе трёх карт шума (высота, влажность, температура)
/// с последующей классификацией биомов.
/// </summary>
/// 
public class TerrainGenerator

{
    
    // Три независимых источника шума. Разные seed'ы, чтобы поля не коррелировали.
    private readonly FastNoiseLite _elevationNoise;
    private readonly FastNoiseLite _humidityNoise;
    private readonly FastNoiseLite _temperatureNoise;

    // Пороги высоты. Вынесены в константы, чтобы легко настраивать.
    private const float WaterLevel = 0.35f;
    private const float MountainLevel = 0.80f;

    public TerrainGenerator(int seed)
    {
        // Каждому шуму — свой seed. +1000 и +2000 дают независимые поля.
        _elevationNoise = CreateFbmNoise(seed, frequency: 0.02f);
        _humidityNoise = CreateFbmNoise(seed + 1000, frequency: 0.03f);
        _temperatureNoise = CreateFbmNoise(seed + 2000, frequency: 0.01f);
    }

    /// <summary>
    /// Создаёт источник fBm-шума (fractal Brownian motion) — несколько слоёв
    /// шума с разной частотой, сложенных вместе. Даёт и крупные формы, и детали.
    /// </summary>
    private static FastNoiseLite CreateFbmNoise(int seed, float frequency)
    {
        var noise = new FastNoiseLite(seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetFractalOctaves(5);         // 5 слоёв — хороший баланс
        noise.SetFractalLacunarity(2.0f);   // каждый слой в 2 раза мельче
        noise.SetFractalGain(0.5f);         // каждый слой в 2 раза слабее
        noise.SetFrequency(frequency);      // крупность: меньше = крупнее объекты
        return noise;
    }

    /// <summary>
    /// Генерирует мир целиком. Заполняет клетки биомами и их параметрами.
    /// </summary>
    public void Generate(World world)
    {
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                // Сырые значения шума в диапазоне примерно [-1, 1].
                float e = _elevationNoise.GetNoise(x, y);
                float h = _humidityNoise.GetNoise(x, y);
                float t = _temperatureNoise.GetNoise(x, y);

                // Нормализуем в [0, 1] — так проще работать с порогами.
                float eNorm = (e + 1f) / 2f;
                float hNorm = (h + 1f) / 2f;
                float tNorm = (t + 1f) / 2f;

                // Определяем биом по комбинации трёх значений.
                TerrainType biome = ClassifyBiome(eNorm, hNorm, tNorm);

                // Записываем в клетку.
                ref var cell = ref world.GetCell(x, y);
                cell.Terrain = biome;
                cell.Elevation = (int)(eNorm * 100);
                cell.Humidity = (int)(hNorm * 100);
                cell.Temperature = (int)(tNorm * 100);
                cell.Resources = 0; // ресурсы пока не генерируем
            }
        }
    }

    /// <summary>
    /// Определяет биом по нормализованным высоте, влажности и температуре.
    /// Сначала проверяем высоту (вода/горы), потом комбинацию влажности и температуры.
    /// </summary>
    private static TerrainType ClassifyBiome(float elevation, float humidity, float temperature)
    {
        // Вода и горы определяются только высотой.
        if (elevation < WaterLevel) return TerrainType.Water;
        if (elevation > MountainLevel) return TerrainType.Mountain;

        // Дальше — матрица биомов по температуре и влажности.
        // Холодно.
        if (temperature < 0.35f)
        {
            if (humidity < 0.40f) return TerrainType.Tundra;
            return TerrainType.Taiga;
        }

        // Умеренно.
        if (temperature < 0.65f)
        {
            if (humidity < 0.35f) return TerrainType.Grassland;
            if (humidity < 0.70f) return TerrainType.Forest;
            return TerrainType.Swamp;
        }

        // Жарко.
        if (humidity < 0.30f) return TerrainType.Desert;
        if (humidity < 0.65f) return TerrainType.Savanna;
        return TerrainType.TropicalRainforest;
    }
}