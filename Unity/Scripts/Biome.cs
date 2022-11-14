using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Biome : ScriptableObject
{
    public string biomeName;
    [Range(0,1)]
    public float heatThreshold = 0.1f;
    [Range(0, 1)]
    public float moistureThreshold = 0.1f;
    [Range(1, 10)]
    public int treeRadius = 1;
}
