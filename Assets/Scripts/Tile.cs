using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile
{
    private static float _averageConnections = 2f;

    public bool HasDoor { get; }
    public Dictionary<Hex, bool> HasConnection { get; private set; }

    public Tile(bool hasDoor)
    {
        HasDoor = hasDoor;
        SetRandomConnections();
    }

    private void SetRandomConnections()
    {
        HasConnection = new Dictionary<Hex, bool>();
        foreach (Hex direction in Hex.directions)
            HasConnection[direction] = Random.value < (_averageConnections / 6);
    }
}
