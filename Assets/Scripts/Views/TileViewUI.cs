using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileViewUI : MonoBehaviour
{
    [SerializeField] private HexEnumToGameObjectDictionary _sides;

    private RectTransform _rectTransform;

    public DeckView DeckView { get; set; }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnClick() => DeckView.OnClickTile(this);

    public void SetPosition(float minY, float maxY)
    {
        _rectTransform.anchorMin = new Vector2(_rectTransform.anchorMin.x, minY);
        _rectTransform.anchorMax = new Vector2(_rectTransform.anchorMax.x, maxY);
    }

    public void SetVisuals(Tile tile)
    {
        foreach (Hex direction in Hex.directions)
            Side(direction).SetActive(tile.HasConnection[direction]);
    }

    public void Select()
    {
        transform.localScale = Vector3.one * 1.4f;
    }

    public void Deselect()
    {
        transform.localScale = Vector3.one;
    }
    private GameObject Side(Hex direction)
        => _sides[direction.ToEnum()];
}

