using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HexDirectionGameObjectDictionary
    : SerializableDictionary<HexDirection, GameObject>
{ }

public class TileView : MonoBehaviour
{
    [SerializeField] private HexDirectionGameObjectDictionary _sides;
    [SerializeField] private GameObject _door;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _content;

    private void Start()
    {
        _highlight.SetActive(false);
    }

    public void SetPosition(Vector2 position)
        => transform.position = position;

    public void SetVisuals(Tile tile)
    {
        _door.SetActive(tile.HasDoor);
        foreach (HexDirection direction in _sides.Keys)
            _sides[direction].SetActive(tile.HasConnection[direction]);
    }

    public void HighlightPlacePossible()
    {
        _content.SetActive(false);
        _highlight.SetActive(true);
    }

    public void HighlightNextPlace()
    {
        _content.SetActive(true);
        _highlight.SetActive(true);
    }
}

