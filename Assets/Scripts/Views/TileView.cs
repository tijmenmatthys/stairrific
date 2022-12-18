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

    private void Start()
    {
        _sides[HexDirection.Left].SetActive(false);
    }

    public void SetPosition(Vector2 position)
        => transform.position = position;

    public void UpdateVisuals(Tile tile)
    {
        _door.SetActive(tile.HasDoor);
        foreach (HexDirection direction in _sides.Keys)
            _sides[direction].SetActive(tile.HasConnection[direction]);
    }
}

