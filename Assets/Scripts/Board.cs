using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public event Action<Hex, Tile> TileAdded;

    private Dictionary<Hex, Tile> _tiles = new Dictionary<Hex,Tile>();
    private List<Hex> _validPositions = new List<Hex>();

    public List<Hex> ValidPositions => _validPositions;

    public void AddTile(Hex position, Tile tile)
    {
        _tiles[position] = tile;
        TileAdded?.Invoke(position, tile);
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
