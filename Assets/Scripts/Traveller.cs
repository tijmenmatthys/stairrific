using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Traveller
{
    public event Action<Vector2> Moved;
    public event Action StartedMoving;
    public event Action StartedWaiting;

    private bool _isMoving = false;
    private readonly Hex _target;
    private Hex _position;
    private Vector2 _worldPosition;

    public bool IsMoving
    {
        get => _isMoving;
        set
        {
            if (_isMoving && !value) StartedWaiting?.Invoke();
            if (!_isMoving && value) StartedMoving?.Invoke();
            _isMoving = value;
        }
    }
    public Vector2 WorldPosition
    {
        get => _worldPosition;
        set
        {
            _worldPosition = value;
            Moved?.Invoke(value);
        }
    }
    public Hex Position => _position;
    public Hex Target => _target;

    public Traveller(Hex position, Hex target)
    {
        _position = position;
        WorldPosition = Position.WorldPosition2D;
        _target = target;
    }

    public void Move(Hex direction)
    {
        _position += direction;
        WorldPosition = Position.WorldPosition2D;
    }
}
