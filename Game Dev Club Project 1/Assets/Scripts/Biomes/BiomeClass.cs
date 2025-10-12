using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class BiomeClass
{
    public string biomeName;
    public BiomeType biome;

    public Color biomeCol;

    public Tile tileSprite;

    public int minNumOfTiles;
    public int numTiles;

}
