using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile
{
    private static float _averageConnections = 2f;
    private static float _minConnections = 2; // don't put higher than 2, otherwise inefficient/endless algorithm

    public bool HasDoor { get; }
    public Dictionary<Hex, bool> HasConnection { get; private set; }
    private int ConnectionCount => HasConnection.Values.Count(b => b);

    public Tile(bool hasDoor)
    {
        HasDoor = hasDoor;
        SetRandomConnections();
    }

    private void SetRandomConnections()
    {
        do
        {
            HasConnection = new Dictionary<Hex, bool>();
            foreach (Hex direction in Hex.directions)
                HasConnection[direction] = Random.value < (_averageConnections / 6);
        } while (ConnectionCount < _minConnections);
    }

    public void Rotate(bool isLeft)
    {
        var newConnections = new Dictionary<Hex, bool>();
        foreach (var direction in Hex.directions)
        {
            var newDirection = !isLeft ? direction.RotateLeft() : direction.RotateRight();
            newConnections.Add(newDirection, HasConnection[direction]);
        }
        HasConnection = newConnections;
    }
}
