using UnityEngine;

namespace TacticsGame.TileGrid
{
    /// <summary>
    /// This class is responsible for creating and querying the runtime Tile Grid.
    /// </summary>
    public class TileGridService
    {
        private readonly Tile _tilePrefab;

        // This array stores references to all generated tiles as coordinates of the grid.
        private Tile[,] _tilesArray;

        // Properties to hold dimensions of the grid & tile size
        public int GridWidth { get; private set; }

        public int GridHeight { get; private set; }

        public float TileSize { get; private set; }

        private readonly float _xOffset;
        private readonly float _zOffset;

        public TileGridService(TileGrid_SO tileGrid_SO)
        {
            _tilePrefab = tileGrid_SO.tilePrefab;

            GridWidth = tileGrid_SO.gridWidth;
            GridHeight = tileGrid_SO.gridHeight;
            TileSize = tileGrid_SO.tileSize;

            _xOffset = (GridWidth - 1) * TileSize / 2f;
            _zOffset = (GridHeight - 1) * TileSize / 2f;
        }

        /// <summary>
        /// This function creates grid of tiles, initializes and register them 
        /// in the tiles array for fast lookup.
        /// </summary>
        public void GenerateTileGrid()
        {
            _tilesArray = new Tile[GridWidth, GridHeight];

            for (int x = 0; x < GridHeight; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    Vector2Int position = new Vector2Int(x, y);

                    Vector3 worldPosition = GetGridToWorldPosition(position);

                    Tile tile = Object.Instantiate(_tilePrefab, worldPosition, Quaternion.identity);
                    tile.Initialize(position);
                    _tilesArray[x, y] = tile;
                }
            }
        }

        /// <summary>
        /// Converts a grid coordinate into a world-space position.
        /// The grid is placed on the X-Z plane while Y represents height.
        /// </summary>
        public Vector3 GetGridToWorldPosition(Vector2Int position)
        {
            return new Vector3(position.x * TileSize - _xOffset, 0f, position.y * TileSize - _zOffset);
        }
    }
}