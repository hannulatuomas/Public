using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MoistureMapData : UpdatableData
{
    public MoistureGradient[] moistureGradients;

    public AnimationCurve moistureCurve;
    [Range(0, 1)]
    public float moistureTerrainHeightEffect = 0.35f;
}

[System.Serializable]
public struct MoistureGradient
{
    public string name;
    public float height;
    public Color colour;
}
