using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DeckView : MonoBehaviour
{
    [SerializeField] private int _visibleTileCount = 5;
    [SerializeField] private int _selectableTileCount = 5;
    [SerializeField] private GameObject _tilePrefab;

    private List<TileViewUI> _tileViews = new List<TileViewUI>();

    public int SelectableTileCount => _selectableTileCount;

    public void OnTileAdded(Tile tile)
    {
        var tileView = Instantiate(_tilePrefab, transform).GetComponent<TileViewUI>();
        _tileViews.Add(tileView);
        tileView.SetVisuals(tile);
        SetPosition(tileView, _tileViews.Count - 1);
    }

    public void OnTileRemoved(int index)
    {
        Destroy(_tileViews[index].gameObject);
        _tileViews.RemoveAt(index);

        // Move position of all the next tiles
        for (int i = index; i < _tileViews.Count; i++)
            SetPosition(_tileViews[i], i);
    }

    public void OnTileSelected(int index)
    {
        for (int i = 0; i < _tileViews.Count; i++)
            _tileViews[i].Deselect();

        if (index >= 0)
            _tileViews[index].Select();
    }

    private void SetPosition(TileViewUI tileView, int index)
    {
        float max = 1 - ((float)index / _visibleTileCount);
        float min = 1 - ((float)(index + 1) / _visibleTileCount);
        tileView.SetPosition(min, max);
    }
}
