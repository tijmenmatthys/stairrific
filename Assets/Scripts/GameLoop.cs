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
        //SetMouseToOrigin();
        InitViews();
        InitModels();
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
        _deck.TileSelected += _deckView.OnTileSelected;
        _deck.AddTiles(_startTileCount);
    }

    public void OnMovePointer(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        var mousePosition = Mouse.current.position.ReadValue();
        _pointerPosition = _camera.ScreenToWorldPoint(mousePosition);
    }

    public void OnPlaceTile(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_deck.TileCount <= 0) return;

        _board.AddTile(Hex.FromWorldPosition(_pointerPosition), _deck.SelectedTile);
        _deck.RemoveTile();
    }

    private void SetMouseToOrigin()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
    }
}
