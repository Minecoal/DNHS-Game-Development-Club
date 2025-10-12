using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGen : MonoBehaviour
{

    [SerializeField] private float seed;

    [SerializeField] private BiomeClass[] biomes;

    [Header("Biomes")]
    [SerializeField] private float biomeFrequency;
    [SerializeField] private Gradient biomeGradient;

    public int size;


    [SerializeField] private Texture2D biomeMap;
    private BiomeClass curBiome;
    [SerializeField] private Tilemap tilemap;

    public List<TileClass> tiles = new List<TileClass>();

    private void Start()
    {
        
        DrawTextures();
        ApplyToTilemap();
    }

    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DrawTextures();
            ApplyToTilemap();
        }
    }

    public void DrawTextures()
    {
        seed = Random.Range(0f, 100000f);

        biomeMap = new Texture2D(size, size);

        for (int x = 0; x < biomeMap.width; x++)
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);
                Color col = biomeGradient.Evaluate(v);
                biomeMap.SetPixel(x, y, col);
            }
        }

        biomeMap.Apply();
    }

    void ApplyToTilemap()
    {
        tilemap.ClearAllTiles();
        foreach (BiomeClass biome in biomes)
        {
            biome.numTiles = 0;
        }

        for (int x = 0; x < biomeMap.width; x++)
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                Color pixelColor = biomeMap.GetPixel(x, y);

                BiomeClass matchedBiome = GetClosestBiome(pixelColor);

                if (matchedBiome != null && matchedBiome.tileSprite != null)
                {
                    matchedBiome.numTiles++;
                    tilemap.SetTile(new Vector3Int(x, y, 0), matchedBiome.tileSprite);
                }
            }
        }

        if (EnoughTiles())
        {
            Debug.Log("world gen successful");
        }
        else
        {
            Debug.Log("redo world gen");
            
        }
    }


    BiomeClass GetClosestBiome(Color col)
    {
        BiomeClass closest = null;
        float minDist = Mathf.Infinity;

        foreach (BiomeClass biome in biomes)
        {
            float dist = Vector3.Distance(
                new Vector3(col.r, col.g, col.b),
                new Vector3(biome.biomeCol.r, biome.biomeCol.g, biome.biomeCol.b)
            );

            if (dist < minDist)
            {
                minDist = dist;
                closest = biome;
            }
        }

        return closest;
    }

    private bool EnoughTiles()
    {
        foreach (BiomeClass biome in biomes)
        {
            if (biome.numTiles < biome.minNumOfTiles)
            {
                return false;
            }
        }
        return true;
    }
}
