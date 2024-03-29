using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField] private GameObject _travellerPrefab;
    [SerializeField] private GameObject _tilePrefab;

    private Dictionary<Traveller, TravellerView> _travellers = new Dictionary<Traveller, TravellerView>();
    private Dictionary<Hex, TileView> _tiles = new Dictionary<Hex, TileView>();
    private Dictionary<Hex, TileView> _highLightedTiles = new Dictionary<Hex, TileView>();

    public void OnTileAdded(Hex position, Tile tile)
    {
        if (!_tiles.ContainsKey(position))
        {
            GameObject newTile = Instantiate(_tilePrefab, gameObject.transform);
            _tiles.Add(position, newTile.GetComponent<TileView>());
        }
        _tiles[position].SetPosition(position.WorldPosition2D);
        _tiles[position].SetVisuals(tile);
    }

    public void OnTileRemoved(Hex position)
    {
        Destroy(_tiles[position].gameObject);
        _tiles.Remove(position);
        if (_highLightedTiles.ContainsKey(position))
            _highLightedTiles.Remove(position);
    }

    public void OnTravellerAdded(Traveller traveller)
    {
        GameObject newTraveller = Instantiate(_travellerPrefab, gameObject.transform);
        var travellerView = newTraveller.GetComponent<TravellerView>();
        travellerView.SetStartPosition(traveller.WorldPosition);
        _travellers.Add(traveller, travellerView);

        traveller.Moved += travellerView.SetPosition;
        traveller.StartedMoving += travellerView.SetMoving;
        traveller.StartedWaiting += travellerView.SetWaiting;
    }

    public void OnTravellerRemoved(Traveller traveller)
    {
        Destroy(_travellers[traveller].gameObject);
        _travellers.Remove(traveller);
    }

    public void HighlightValid(List<Hex> validPositions, Tile nextTile)
    {
        foreach (Hex position in _highLightedTiles.Keys)
            Destroy(_highLightedTiles[position].gameObject);
        _highLightedTiles.Clear();

        foreach (Hex position in validPositions)
        {
            TileView highlightTile = Instantiate(_tilePrefab, gameObject.transform).GetComponent<TileView>();
            highlightTile.SetPosition(position.WorldPosition2D);
            highlightTile.SetVisuals(nextTile);
            highlightTile.HighlightValid();
            _highLightedTiles.Add(position, highlightTile);
        }
    }

    public void HighlightNextPlace(Hex nextPlacePosition)
    {
        foreach (Hex position in _highLightedTiles.Keys)
            _highLightedTiles[position].HighlightValid();
        _highLightedTiles[nextPlacePosition].HighlightNextPlace();
    }
}
