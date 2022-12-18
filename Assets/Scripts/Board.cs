using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public event Action<Hex, Tile> TileAdded;

    private Dictionary<Hex, Tile> _tiles = new Dictionary<Hex,Tile>();

    public void AddTile(Hex position, Tile tile)
    {
        _tiles[position] = tile;
        TileAdded?.Invoke(position, tile);
    }
}
