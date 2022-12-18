using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Deck
{
    public event Action<Tile> TileAdded;
    public event Action<int> TileRemoved;
    public event Action<int> TileSelected;

    private int _selectableTileCount;
    private List<Tile> _deck = new List<Tile>();
    private int _selectedTileIndex = -1;

    public Tile SelectedTile => _deck[SelectedTileIndex];
    public int TileCount => _deck.Count;
    private int SelectedTileIndex
    {
        get => _selectedTileIndex;
        set
        {
            _selectedTileIndex = value;
            TileSelected?.Invoke(SelectedTileIndex);
        }
    }

    public Deck(int selectableTileCount)
    {
        _selectableTileCount = selectableTileCount;
    }

    public void AddTiles(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var tile = new Tile(false);
            _deck.Add(tile);
            TileAdded?.Invoke(tile);

            if (_deck.Count == 1)
                SelectedTileIndex = 0;
        }
    }

    public void RemoveTile()
    {
        _deck.RemoveAt(SelectedTileIndex);
        TileRemoved?.Invoke(SelectedTileIndex);

        if (SelectedTileIndex == _deck.Count)
            SelectedTileIndex--;
        else
            SelectedTileIndex = SelectedTileIndex;
    }

    public void SelectNextTile()
    {
        int nextIndex = SelectedTileIndex + 1;
        if (nextIndex < _selectableTileCount
            && nextIndex < TileCount)
            SelectedTileIndex = nextIndex;
    }
    public void SelectPreviousTile()
    {
        int previousIndex = SelectedTileIndex - 1;
        if (previousIndex >= 0)
            SelectedTileIndex = previousIndex;
    }
}
