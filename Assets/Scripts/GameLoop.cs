using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameLoop : MonoBehaviour
{
    private BoardView _boardView;

    private Board _board;

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
    }

    private void InitModels()
    {
        _board = new Board();
        _board.TileAdded += _boardView.OnTileAdded;
    }

    public void OnMovePointer(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        var mousePosition = Mouse.current.position.ReadValue();
        _pointerPosition = _camera.ScreenToWorldPoint(mousePosition);
        Debug.Log(_pointerPosition);
    }

    public void OnPlaceTile(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        _board.AddTile(Hex.FromWorldPosition(_pointerPosition), new Tile(false));
    }

    private void SetMouseToOrigin()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
    }
}
