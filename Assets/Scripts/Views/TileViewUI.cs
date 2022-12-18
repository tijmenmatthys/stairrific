using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileViewUI : MonoBehaviour
{
    [SerializeField] private HexDirectionGameObjectDictionary _sides;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetPosition(float minY, float maxY)
    {
        _rectTransform.anchorMin = new Vector2(_rectTransform.anchorMin.x, minY);
        _rectTransform.anchorMax = new Vector2(_rectTransform.anchorMax.x, maxY);
    }

    public void SetVisuals(Tile tile)
    {
        foreach (HexDirection direction in _sides.Keys)
            _sides[direction].SetActive(tile.HasConnection[direction]);
    }

    public void Select()
    {
        transform.localScale = Vector3.one * 1.3f;
    }

    public void Deselect()
    {
        transform.localScale = Vector3.one;
    }
}

