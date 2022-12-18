using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile
{
    private static float _averageConnections = 2f;

    public bool HasDoor { get; }
    public Dictionary<HexDirection, bool> HasConnection { get; private set; }

    public Tile(bool hasDoor)
    {
        HasDoor = hasDoor;
        SetRandomConnections();
    }

    private void SetRandomConnections()
    {
        HasConnection = new Dictionary<HexDirection, bool>();
        foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
            HasConnection[direction] = Random.value < (_averageConnections / 6);
    }
}
