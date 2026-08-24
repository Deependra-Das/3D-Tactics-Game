using System.Collections.Generic;
using UnityEngine;
using TacticsGame.TileGrid;

namespace TacticsGame.PathFinding
{
    /// <summary>
    /// This class handles grid-based pathfinding for units.
    /// Breadth-First Search (BFS) is used because every movement
    /// from one tile to an adjacent tile has the same cost.
    /// </summary>
    public class PathfindingService
    {
        private readonly TileGridService _tileGridService;

        public PathfindingService(TileGridService tileGridService)
        {
            _tileGridService = tileGridService;
        }

        /// <summary>
        /// This function finds the shortest path between two grid positions
        /// using Breadth-First Search.
        /// </summary>
        public List<Vector2Int> FindShortestPath(Vector2Int startPosition, Vector2Int targetPosition)
        {
            if (!_tileGridService.IsInsideGrid(startPosition))
            {
                return null;
            }

            if (!_tileGridService.IsInsideGrid(targetPosition))
            {
                return null;
            }

            if (!_tileGridService.IsWalkable(targetPosition))
            {
                return null;
            }

            // If the unit is already at the destination,
            // no movement is required.
            if (startPosition == targetPosition)
            {
                return new List<Vector2Int>();
            }

            // Stores tiles to be explored.
            Queue<Vector2Int> tilesToExplore = new Queue<Vector2Int>();

            // Stores the previous tile used to reach each discovered tile.
            Dictionary<Vector2Int, Vector2Int> previousTiles = new Dictionary<Vector2Int, Vector2Int>();

            tilesToExplore.Enqueue(startPosition);
            previousTiles[startPosition] = startPosition;

            // Keep searching while there are tiles that remain to be explored.
            while (tilesToExplore.Count > 0)
            {
                Vector2Int currentPosition =
                    tilesToExplore.Dequeue();

                // The target has been reached.
                if (currentPosition == targetPosition)
                {
                    return BuildPath(previousTiles, startPosition, targetPosition);
                }

                // Check all four neighbouring tiles.
                foreach ( Vector2Int direction in GridDirections.MovementDirections)
                {
                    Vector2Int neighbourPosition = currentPosition + direction;

                    // Ignore positions outside the grid.
                    if (!_tileGridService.IsInsideGrid(neighbourPosition))
                    {
                        continue;
                    }

                    // The target cannot be reached if it is blocked
                    // by an obstacle or occupied by another unit.
                    if (!_tileGridService.IsWalkable(neighbourPosition))
                    {
                        continue;
                    }

                    // Ignore tiles that have already been discovered.
                    if (previousTiles.ContainsKey(neighbourPosition))
                    {
                        continue;
                    }

                    // Remember which tile led to this neighbour.
                    previousTiles[neighbourPosition] = currentPosition;

                    // Add the neighbour to the search queue.
                    tilesToExplore.Enqueue(neighbourPosition);
                }
            }

            // Return Null if the target cannot be reached from the starting tile.
            return null;
        }

        /// <summary>
        /// This Function builds the final path after BFS has found the target.
        /// The previousTiles dictionary contains the information
        /// required to walk backwards from the target to the start.
        /// </summary>
        private List<Vector2Int> BuildPath(Dictionary<Vector2Int, Vector2Int> previousTiles, Vector2Int startPosition, Vector2Int targetPosition)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            Vector2Int currentPosition = targetPosition;

            // Walk backwards from the target until we reach the starting position.
            while (currentPosition != startPosition)
            {
                path.Add(currentPosition);

                currentPosition = previousTiles[currentPosition];
            }

            // The path was created from target to start tile, so reversing it to get start to target path.
            path.Reverse();

            return path;
        }
    }
}