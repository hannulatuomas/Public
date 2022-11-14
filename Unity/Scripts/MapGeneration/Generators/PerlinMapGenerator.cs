using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinMapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap, TerrainHeightMap, HeatMap, MoistureMap, BiomeMap, StructureMap, ResourceMap, PathFindingMap, RoofMap
    }

    public HeightMapData heightMapData;
    public HeatMapData heatMapData;
    public MoistureMapData moistureMapData;
    public RiverGenerator riverGenerator = new RiverGenerator();
    public bool addRivers;

    public PerlinNoiseData[] perlinNoisesHeight;
    public PerlinNoiseData[] perlinNoisesHeat;
    public PerlinNoiseData[] perlinNoisesMoisture;

    public bool autoUpdate;
    public DrawMode drawMode;
    public bool showRivers = true;

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            GeneratePerlinMap();
        }
    }

    public void GeneratePerlinMap()
    {
        float[,] noiseMap = new float[MapBase.SizeX, MapBase.SizeY];
        float[,] terrainHeightMap = new float[MapBase.SizeX, MapBase.SizeY];
        // FIX ME! Cut to separate functions!!
        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                noiseMap = GenerateNoiseMap.GenerateRandomNoiseMap(MapBase.SizeX, MapBase.SizeY, perlinNoisesHeight);
                if (heightMapData.addCoast && heightMapData.coastDistance > 0)
                {
                    noiseMap = AddCoast.AddCoastToNoisemap(noiseMap, heightMapData.coastDistance, heightMapData.coastDirection);
                }
                if (addRivers)
                {
                    noiseMap = riverGenerator.GenerateRivers(noiseMap);
                }
                break;
            case DrawMode.TerrainHeightMap:
                noiseMap = GenerateNoiseMap.GenerateRandomNoiseMap(MapBase.SizeX, MapBase.SizeY, perlinNoisesHeight);
                if (heightMapData.addCoast && heightMapData.coastDistance > 0)
                {
                    noiseMap = AddCoast.AddCoastToNoisemap(noiseMap, heightMapData.coastDistance, heightMapData.coastDirection);
                }
                MapBase.HeightMapBase = noiseMap;
                if (addRivers)
                {
                    noiseMap = riverGenerator.GenerateRivers(noiseMap);
                }
                break;
            case DrawMode.HeatMap:
                noiseMap = GenerateNoiseMap.GenerateUniformNoiseMap(MapBase.SizeX, MapBase.SizeY, heatMapData.heatOffset);
                terrainHeightMap = GenerateNoiseMap.GenerateRandomNoiseMap(MapBase.SizeX, MapBase.SizeY, perlinNoisesHeight);

                float[,] noiseMap2 = GenerateNoiseMap.GenerateRandomNoiseMap(MapBase.SizeX, MapBase.SizeY, perlinNoisesHeat);
                if (heightMapData.addCoast && heightMapData.coastDistance > 0)
                {
                    terrainHeightMap = AddCoast.AddCoastToNoisemap(terrainHeightMap, heightMapData.coastDistance, heightMapData.coastDirection);
                }
                for (int i = 0; i < noiseMap.GetLength(0); i++)
                {
                    for (int j = 0; j < noiseMap.GetLength(1); j++)
                    {
                        noiseMap[i, j] = (1 - heatMapData.heatRandomEffect) * noiseMap[i, j] + heatMapData.heatRandomEffect * noiseMap2[i, j];
                        noiseMap[i, j] = Mathf.Clamp((1 - heatMapData.heatTerrainHeightEffect) * noiseMap[i, j] + heatMapData.heatTerrainHeightEffect * (1 - heatMapData.heatCurve.Evaluate(terrainHeightMap[i, j])), 0f, 1f);
                    }
                }
                break;
            case DrawMode.MoistureMap:
                terrainHeightMap = GenerateNoiseMap.GenerateRandomNoiseMap(MapBase.SizeX, MapBase.SizeY, perlinNoisesHeight);
                noiseMap = GenerateNoiseMap.GenerateRandomNoiseMap(MapBase.SizeX, MapBase.SizeY, perlinNoisesMoisture);

                if (heightMapData.addCoast)
                {
                    terrainHeightMap = AddCoast.AddCoastToNoisemap(terrainHeightMap, heightMapData.coastDistance, heightMapData.coastDirection);
                }
                for (int i = 0; i < noiseMap.GetLength(0); i++)
                {
                    for (int j = 0; j < noiseMap.GetLength(1); j++)
                    {
                        noiseMap[i, j] = Mathf.Clamp((1 - moistureMapData.moistureTerrainHeightEffect) * noiseMap[i, j] + moistureMapData.moistureTerrainHeightEffect * moistureMapData.moistureCurve.Evaluate(terrainHeightMap[i, j]), 0f, 1f);
                    }
                }
                break;
            case DrawMode.BiomeMap:

                break;
            case DrawMode.StructureMap:
                break;
            case DrawMode.ResourceMap:
                break;
            case DrawMode.PathFindingMap:
                break;
            default:
                break;
        }

        Color[] colourMap = new Color[MapBase.SizeX * MapBase.SizeY];

        for (int y = 0; y < MapBase.SizeY; y++)
        {
            for (int x = 0; x < MapBase.SizeX; x++)
            {
                float currentHeight = noiseMap[x,y];

                if (drawMode == DrawMode.TerrainHeightMap)
                {
                    for (int i = 0; i < heightMapData.terrainHeightGradients.Length; i++)
                    {
                        if (currentHeight <= heightMapData.terrainHeightGradients[i].height)
                        {
                            colourMap[y * MapBase.SizeX + x] = heightMapData.terrainHeightGradients[i].colour;
                            break;
                        }
                    }
                }
                else if (drawMode == DrawMode.HeatMap)
                {
                    for (int i = 0; i < heatMapData.heatGradients.Length; i++)
                    {
                        if (currentHeight <= heatMapData.heatGradients[i].height)
                        {
                            colourMap[y * MapBase.SizeX + x] = heatMapData.heatGradients[i].colour;
                            break;
                        }
                    }
                }
                else if (drawMode == DrawMode.MoistureMap)
                {
                    for (int i = 0; i < moistureMapData.moistureGradients.Length; i++)
                    {
                        if (currentHeight <= moistureMapData.moistureGradients[i].height)
                        {
                            colourMap[y * MapBase.SizeX + x] = moistureMapData.moistureGradients[i].colour;
                            break;
                        }
                    }
                }

            }
        }

        PerlinMapDisplay display = FindObjectOfType<PerlinMapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawMapTexture(MapTextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.TerrainHeightMap || drawMode == DrawMode.HeatMap || drawMode == DrawMode.MoistureMap)
        {
            if (showRivers)
            {
                for (int i = 0; i < riverGenerator.riverPixels.Count; i++)
                {
                    colourMap[(int)riverGenerator.riverPixels[i].y * MapBase.SizeX + (int)riverGenerator.riverPixels[i].x] = riverGenerator.riverColor;
                }
            }
            display.DrawMapTexture(MapTextureGenerator.TextureFromColourMap(colourMap, MapBase.SizeX, MapBase.SizeY));
        }
    }
    private void OnValidate()
    {
        for (int i = 0; i < perlinNoisesHeight.Length; i++)
        {
            if (perlinNoisesHeight[i] != null)
            {
                perlinNoisesHeight[i].OnValuesUpdated -= OnValuesUpdated;
                perlinNoisesHeight[i].OnValuesUpdated += OnValuesUpdated;
            }
        }
        for (int i = 0; i < perlinNoisesHeat.Length; i++)
        {
            if (perlinNoisesHeight[i] != null)
            {
                perlinNoisesHeat[i].OnValuesUpdated -= OnValuesUpdated;
                perlinNoisesHeat[i].OnValuesUpdated += OnValuesUpdated;
            }
        }
        for (int i = 0; i < perlinNoisesMoisture.Length; i++)
        {
            if (perlinNoisesHeight[i] != null)
            {
                perlinNoisesMoisture[i].OnValuesUpdated -= OnValuesUpdated;
                perlinNoisesMoisture[i].OnValuesUpdated += OnValuesUpdated;
            }
        }
        if (heightMapData != null)
        {
            heightMapData.OnValuesUpdated -= OnValuesUpdated;
            heightMapData.OnValuesUpdated += OnValuesUpdated;
        }
        if (heatMapData != null)
        {
            heatMapData.OnValuesUpdated -= OnValuesUpdated;
            heatMapData.OnValuesUpdated += OnValuesUpdated;
        }
        if (moistureMapData != null)
        {
            moistureMapData.OnValuesUpdated -= OnValuesUpdated;
            moistureMapData.OnValuesUpdated += OnValuesUpdated;
        }
    }
}