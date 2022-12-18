using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HexEnumToGameObjectDictionary
    : SerializableDictionary<HexEnum, GameObject>
{ }

public class TileView : MonoBehaviour
{
    [SerializeField] private HexEnumToGameObjectDictionary _sides;
    [SerializeField] private GameObject _door;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _content;

    private void Awake()
    {
        _highlight.SetActive(false);
    }

    public void SetPosition(Vector2 position)
        => transform.position = position;

    public void SetVisuals(Tile tile)
    {
        _door.SetActive(tile.HasDoor);
        foreach (Hex direction in Hex.directions)
            Side(direction).SetActive(tile.HasConnection[direction]);
    }

    public void HighlightValid()
    {
        _content.SetActive(false);
        _highlight.SetActive(true);
    }

    public void HighlightNextPlace()
    {
        _content.SetActive(true);
        _content.transform.localScale = Vector3.one * .7f;
        _highlight.SetActive(false);
    }

    private GameObject Side(Hex direction)
        => _sides[direction.ToEnum()];
}

