using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PerlinNoiseData : UpdatableData
{
    public int seed = 1;
    [SerializeField]
    private float scale = 0.3f;

    public Vector2 offset;

    public float amplitude = 1;         // Starting value, first octave
    public float frequency = 1;         // Starting value, first octave
    [Range(1, 10)]
    public int octaves = 4;
    [Range(0, 1)]
    public float persistance = 0.5f;    // How much amplitude will be in next octave A x P (decrease)
    [SerializeField]
    private float lacunarity = 2f;              // How much frequency will be in next octave F x L (increase)

    public float Scale
    {
        get { return scale; }
        set
        {
            if (value != 0)
            {
                scale = value;
            }
            else
            {
                scale = 0.0001f;
            }
        }
    }
    public float Lacunarity
    {
        get { return lacunarity; }
        set
        {
            if (value >= 1)
            {
                lacunarity = value;
            }
            else
            {
                lacunarity = 1;
            }
        }
    }

}