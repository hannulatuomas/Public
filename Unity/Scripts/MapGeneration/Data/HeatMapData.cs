using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HeatMapData : UpdatableData
{
    public HeatGradient[] heatGradients;

    public AnimationCurve heatCurve;
    [Range(0, 1)]
    public float heatRandomEffect = 0.65f;
    [Range(0, 1)]
    public float heatTerrainHeightEffect = 0.4f;
    public float heatOffset = 0f;
}

[System.Serializable]
public struct HeatGradient
{
    public string name;
    public float height;
    public Color colour;
}