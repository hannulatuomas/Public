using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapGrid
{
    public static int sizeX = 100; // RimWorld 275x275
    public static int sizeY = 100;

    static Tile[,] tiles;

    public static void CreateMapGrid()
    {
        tiles = new Tile[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                tiles[x, y] = new Tile(x,y);
                tiles[x, y].CreateTileGameObject();

                /*
                if (i > 99 || j > 99)
                {
                    tiles[x, y].tileGameObject.SetActive(false);
                }
                */
            }
        }
    }

    static Texture2D GenerateMap()
    {
        Texture2D map = new Texture2D(sizeX, sizeY);

        return map;
    }
}
