using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapBase
{
    private static int sizeX = MapGrid.sizeX*3;
    private static int sizeY = MapGrid.sizeY*3;
    private static float[,] heightMapBase;

    public static int SizeX
    {
        get { return sizeX; }
        set
        {
            if (value > 0)
            {
                sizeX = value;
            }
            else
            {
                sizeX = 1;
            }
        }
    }
    public static int SizeY
    {
        get { return sizeY; }
        set
        {
            if (value > 0)
            {
                sizeY = value;
            }
            else
            {
                sizeY = 1;
            }
        }
    }
    public static float[,] HeightMapBase
    {
        get { return heightMapBase; }
        set
        {
            heightMapBase = value;
        }
    }
}
