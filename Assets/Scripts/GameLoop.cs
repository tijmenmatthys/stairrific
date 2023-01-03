using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameLoop : MonoBehaviour
{
    [SerializeField] private int _startTileCount = 10;
    [SerializeField] private float _travellerMoveInterval = 2f;
    [SerializeField] private float _travellerSpawnInterval = 4f;

    private BoardView _boardView;
    private DeckView _deckView;

    private Board _board;
    private Deck _deck;
    private TravellerMovement _travellerMovement;
    private TravellerNavigation _travellerNavigation;

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

        StartCoroutine(SpawnTravellers());
        StartCoroutine(MoveTravellers());
    }

    private IEnumerator SpawnTravellers()
    {
        while (true)
        {
            Hex source = _board.GetRandomDoor();
            Hex target;
            do { target = _board.GetRandomDoor(); }
            while (target == source);
            _travellerMovement.AddTraveller(source, target);

            yield return new WaitForSeconds(_travellerSpawnInterval);
        }
    }

    private IEnumerator MoveTravellers()
    {
        while (true)
        {
            _travellerMovement.Move();

            yield return new WaitForSeconds(_travellerMoveInterval);
        }
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
        _board.TileAdded += (_, _)
            => _travellerNavigation.UpdateShortestPaths(_board.Tiles, _board.DoorPositions);
        _board.TileRemoved += _boardView.OnTileRemoved;
        _board.TileRemoved += (_)
            => _travellerNavigation.UpdateShortestPaths(_board.Tiles, _board.DoorPositions);

        _deck = new Deck(_deckView.SelectableTileCount);
        _deck.TileAdded += _deckView.OnTileAdded;
        _deck.TileRemoved += _deckView.OnTileRemoved;
        _deck.TileSelected += OnTileSelected;
        _deckView.SelectTile += _deck.SelectTile;

        _travellerNavigation = new TravellerNavigation();
        _travellerMovement = new TravellerMovement(_travellerNavigation);
        _travellerMovement.TravellerAdded += _boardView.OnTravellerAdded;
        _travellerMovement.TravellerRemoved += _boardView.OnTravellerRemoved;
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
        if (IsMouseAboveUI()) return;
        if (_deck.TileCount <= 0) return;
        if (!_board.TryGetNextPlacePosition(_pointerPosition, out Hex nextPlacePosition)) return;

        _board.AddTile(nextPlacePosition, _deck.SelectedTile);
        _deck.RemoveTile();
    }

    public void OnRemoveTile(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        _board.RemoveTile(Hex.FromWorldPosition(_pointerPosition));
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

    private bool IsMouseAboveUI()
    {
        var viewportPosition = _camera.WorldToViewportPoint(_pointerPosition);
        return viewportPosition.x < .2;
    }
}
