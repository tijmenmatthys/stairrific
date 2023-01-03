using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board
{
    public event Action<Hex, Tile> TileAdded;
    public event Action<Hex> TileRemoved;

    private Dictionary<Hex, Tile> _tiles = new Dictionary<Hex,Tile>();
    private List<Hex> _doorPositions = new List<Hex>();
    private List<Hex> _validPositions = new List<Hex>();
    private List<Vector2> _validWorldPositions = new List<Vector2>();

    public List<Hex> ValidPositions => _validPositions;
    public List<Hex> DoorPositions => _doorPositions;
    public Dictionary<Hex, Tile> Tiles => _tiles;

    public Hex GetRandomDoor()
    {
        int index = Random.Range(0, _doorPositions.Count);
        return _doorPositions[index];
    }

    public void AddTile(Hex position, Tile tile)
    {
        _tiles[position] = tile;
        if (tile.HasDoor)
            _doorPositions.Add(position);
        TileAdded?.Invoke(position, tile);
    }

    public bool RemoveTile(Hex position)
    {
        if (!_tiles.ContainsKey(position)) return false;
        if (_tiles[position].HasDoor) return false;

        _tiles.Remove(position);
        TileRemoved?.Invoke(position);
        return true;
    }

    public void UpdateValidPositions(Tile nextTile)
    {
        List<Hex> perimeter = GetPerimeter();
        List<Hex> validPositions = new List<Hex>();
        foreach (Hex position in perimeter)
        {
            bool success = true;
            foreach (Hex direction in Hex.directions)
            {
                if (!_tiles.ContainsKey(position + direction)) continue;

                var neighbour = _tiles[position + direction];
                if (nextTile.HasConnection[direction]
                    != neighbour.HasConnection[-direction])
                    success = false;
            }
            if (success) validPositions.Add(position);
        }

        _validPositions = validPositions;
        _validWorldPositions = validPositions.Select(hex => hex.WorldPosition2D).ToList();
    }

    public bool TryGetNextPlacePosition(Vector2 pointerPosition, out Hex nextPlacePosition)
    {
        nextPlacePosition = Hex.zero;
        if (_validPositions.Count == 0) return false;

        float minDistance = float.MaxValue;
        foreach (Vector2 position in _validWorldPositions)
        {
            float distance = Vector2.Distance(position, pointerPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                nextPlacePosition = Hex.FromWorldPosition(position);
            }
        }
        return true;
    }

    private List<Hex> GetPerimeter()
    {
        List<Hex> perimeter = new List<Hex>();
        List<Hex> existing = new List<Hex>(_tiles.Keys);
        foreach (Hex tile in existing)
            foreach (Hex neighbour in tile.Neighbours)
            {
                if (perimeter.Contains(neighbour)) continue;
                if (existing.Contains(neighbour)) continue;
                perimeter.Add(neighbour);
            }
        return perimeter;
    }
}
