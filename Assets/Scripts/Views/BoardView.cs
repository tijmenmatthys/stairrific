using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField] private GameObject TilePrefab;

    private Dictionary<Hex, TileView> _tiles = new Dictionary<Hex, TileView>();

    public void OnTileAdded(Hex position, Tile tile)
    {
        if (!_tiles.ContainsKey(position))
        {
            GameObject newTile = Instantiate(TilePrefab, gameObject.transform);
            _tiles.Add(position, newTile.GetComponent<TileView>());
        }
        _tiles[position].SetPosition(position.WorldPosition2D);
        _tiles[position].SetVisuals(tile);
    }
}
