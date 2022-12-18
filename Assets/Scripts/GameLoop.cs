using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameLoop : MonoBehaviour
{
    [SerializeField] private int _startTileCount = 10;

    private BoardView _boardView;
    private DeckView _deckView;

    private Board _board;
    private Deck _deck;

    private Vector2 _pointerPosition = Vector2.zero;
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;

        InitViews();
        InitModels();

        _board.AddTile(Hex.zero, new Tile(true));
        _board.AddTile(new Hex(-3, -1), new Tile(true));
        _board.AddTile(new Hex(2, 2), new Tile(true));
        _deck.AddTiles(_startTileCount);
    }

    private void InitViews()
    {
        _boardView = FindObjectOfType<BoardView>();
        _deckView = FindObjectOfType<DeckView>();
    }

    private void InitModels()
    {
        _board = new Board();
        _board.TileAdded += _boardView.OnTileAdded;

        _deck = new Deck(_deckView.SelectableTileCount);
        _deck.TileAdded += _deckView.OnTileAdded;
        _deck.TileRemoved += _deckView.OnTileRemoved;
        _deck.TileSelected += OnTileSelected;
    }

    public void OnTileSelected(int index)
    {
        _board.UpdateValidPositions(_deck.SelectedTile);
        _boardView.HighlightValid(_board.ValidPositions, _deck.SelectedTile);
        _deckView.OnTileSelected(index);

    }

    public void OnMovePointer(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        var mousePosition = Mouse.current.position.ReadValue();
        _pointerPosition = _camera.ScreenToWorldPoint(mousePosition);

        if (_board.TryGetNextPlacePosition(_pointerPosition, out Hex nextPlacePosition))
            _boardView.HighlightNextPlace(nextPlacePosition);
    }

    public void OnPlaceTile(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_deck.TileCount <= 0) return;
        if (!_board.TryGetNextPlacePosition(_pointerPosition, out Hex nextPlacePosition)) return;

        _board.AddTile(nextPlacePosition, _deck.SelectedTile);
        _deck.RemoveTile();
    }

    public void OnSelectPreviousTile(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _deck.SelectPreviousTile();
    }

    public void OnSelectNextTile(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _deck.SelectNextTile();
    }
}
