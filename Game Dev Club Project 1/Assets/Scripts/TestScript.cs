using UnityEngine;

public class TestScript : MonoBehaviour
{
    Grid<Tile> grid;

    void Awake()
    {
        grid = new Grid<Tile>(5, 5, 1f, (Grid<Tile> g, int x, int y) => new Tile(g, x, y), Vector3.zero);
        grid.CreateDebugText();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public class Tile
    {
        public Grid<Tile> grid;
        public int x;
        public int y;
        public TileType tileType;

        public Tile(Grid<Tile> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            tileType = TileType.Stone;
        }

        public override string ToString()
        {
            return tileType.ToString();
        }
    }

    public enum TileType
    {
        Grass,
        Dirt,
        Stone
    }
}
