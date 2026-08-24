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
        /// This function converts a grid coordinate into a world-space position.
        /// The grid is placed on the X-Z plane while Y represents height.
        /// </summary>
        public Vector3 GetGridToWorldPosition(Vector2Int position)
        {
            return new Vector3(position.x * TileSize - _xOffset, 0f, position.y * TileSize - _zOffset);
        }

        /// <summary>
        /// This function converts a world-space position into a grid coordinate.
        /// </summary> 
        public Vector2Int GetWorldToGridPosition(Vector3 worldPosition)
        {
            return new Vector2Int( Mathf.RoundToInt((worldPosition.x + _xOffset) / TileSize),
                Mathf.RoundToInt((worldPosition.z + _zOffset) / TileSize));
        }

        /// <summary>
        /// This function returns the tile at the requested grid position.
        /// </summary>
        public Tile GetTile(Vector2Int position)
        {
            if (!IsInsideGrid(position))
                return null;

            return _tilesArray[position.x, position.y];
        }

        /// <summary>
        /// This function determines whether a coordinate belongs to the tile grid.
        /// </summary>
        public bool IsInsideGrid(Vector2Int position)
        {
            return position.x >= 0 && position.x < GridWidth && position.y >= 0 && position.y < GridHeight;
        }

        /// <summary>
        /// This function checks the runtime blocked state of a tile & return its value.
        /// </summary>
        public bool IsBlocked(Vector2Int position)
        {
            Tile tile = GetTile(position);
            if (tile == null)
                return true;

            return tile.IsBlocked;
        }

        /// <summary>
        /// This function checks the runtime occupied state of a tile & return its value.
        /// </summary>
        public bool IsOccupied(Vector2Int position)
        {
            Tile tile = GetTile(position);
            if (tile == null)
                return true;

            return tile.IsOccupied;
        }

        /// <summary>
        /// This function determines whether a tile can currently be traversed.
        /// </summary>
        public bool IsWalkable(Vector2Int position)
        {
            Tile tile = GetTile(position);

            if (tile == null)
            {
                return false;
            }

            return !tile.IsBlocked && !tile.IsOccupied;
        }
    }
}