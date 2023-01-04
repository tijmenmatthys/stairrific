using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameLoop : MonoBehaviour
{
    [SerializeField] private int _startTileCount = 10;
    [SerializeField] private int _startDoorCount = 3;
    [SerializeField] private int _minUnconnectedDoors = 2;
    [SerializeField] private float _travellerMoveInterval = 2f;
    [SerializeField] private float _travellerSpawnInterval = 4f;
    [SerializeField] private float _cameraPanSpeed = 5f;
    [SerializeField] private InputActionReference _cameraPan;

    private BoardView _boardView;
    private DeckView _deckView;
    private HUDView _hudView;

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

        for (int i = 0; i < _startDoorCount; i++)
            _board.AddDoor();
        _deck.AddTiles(_startTileCount);

        StartCoroutine(SpawnTravellers());
        StartCoroutine(MoveTravellers());
    }

    private void Update()
    {
        PanCamera();
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
        _hudView = FindObjectOfType<HUDView>();
    }

    private void InitModels()
    {
        _board = new Board();
        _board.TileAdded += _boardView.OnTileAdded;
        _board.TileAdded += (_, _)
            => _travellerNavigation.UpdateShortestPaths(_board.Tiles, _board.DoorPositions);
        _board.TileAdded += (_,t) => UpdateDoors(t);
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

    private void UpdateDoors(Tile latestTile)
    {
        if (latestTile.HasDoor) return;

        // if there are not enough unconnected doors, add new ones
        int doorCount = _board.DoorPositions.Count;
        int connectedDoorCount = _travellerNavigation.GetConnectedDoorCount();
        int newDoorCount = Math.Max(0, connectedDoorCount - doorCount + _minUnconnectedDoors);
        Debug.Log($"Total doors: {doorCount}, connected: {connectedDoorCount}, new needed: {newDoorCount}");
        for (int i = 0; i < newDoorCount; i++)
            _board.AddDoor();
        _hudView.UpdateDoorsConnectedText(connectedDoorCount);
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

    public void PanCamera()
    {
        Vector3 movement = _cameraPan.action.ReadValue<Vector2>();
        _camera.transform.position += movement * _cameraPanSpeed * Time.deltaTime;
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
        _deck.SelectTile(_deck.SelectedTileIndex); // update highlights
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
