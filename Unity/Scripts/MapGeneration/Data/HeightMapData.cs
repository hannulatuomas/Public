using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HeightMapData : UpdatableData
{
    public enum CoastDirection
    {
        North, East, South, West
    }
    
    int sizeX = MapBase.SizeX;
    int sizeY = MapBase.SizeY;

    public TerrainHeightGradient[] terrainHeightGradients;

    public bool addCoast = false;
    public CoastDirection coastDirection;
    public int coastDistance = 10;

}

[System.Serializable]
public struct TerrainHeightGradient
{
    public string name;
    public float height;
    public Color colour;
}