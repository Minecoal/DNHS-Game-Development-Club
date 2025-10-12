using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour
{

    public float seed;

    public BiomeClass[] biomes;

    [Header("Biomes")]
    public float biomeFrequency;
    public Gradient biomeGradient;

    public int size;


    public Texture2D biomeMap;
    private BiomeClass curBiome;

    public List<TileClass> tiles = new List<TileClass>();

    private void Start()
    {
        
        DrawTextures();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DrawTextures();
        }
    }

}
