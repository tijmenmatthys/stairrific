using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TravellerNavigation
{
    // first key is the target door, second key is the current position
    private Dictionary<Hex, Dictionary<Hex, Hex>> _directions;
    private Dictionary<Hex, Dictionary<Hex, int>> _distances;

    public bool TryGetDirection(Hex current, Hex target, out Hex direction)
    {
        direction = Hex.zero;
        if (!_directions.ContainsKey(target)) return false;
        if (!_directions[target].ContainsKey(current)) return false;

        direction = _directions[target][current];
        return true;
    }

    public bool TryGetDistance(Hex current, Hex target, out int distance)
    {
        distance = 0;
        if (!_distances.ContainsKey(target)) return false;
        if (!_distances[target].ContainsKey(current)) return false;

        distance = _distances[target][current];
        return true;
    }

    public int GetConnectedDoorCount()
    {
        int count = 0;
        foreach (Hex target in _directions.Keys)
        {
            bool isConnected = false;
            foreach (Hex source in _directions[target].Keys)
            {
                if (source != target && _directions.ContainsKey(source))
                {
                    isConnected = true;
                    break;
                }
            }
            if (isConnected) count++;
        }
        return count;
    }

    public void UpdateShortestPaths(Dictionary<Hex, Tile> tiles, List<Hex> doors)
    {
        _directions = new Dictionary<Hex, Dictionary<Hex, Hex>>();
        _distances = new Dictionary<Hex, Dictionary<Hex, int>>();
        foreach (Hex door in doors)
            Dijkstra(tiles, door);
    }

    private void Dijkstra(Dictionary<Hex, Tile> tiles, Hex target)
    {
        _directions[target] = new Dictionary<Hex, Hex>();
        _distances[target] = new Dictionary<Hex, int>();
        List<Hex> visited = new List<Hex>();
        List<Hex> queue = new List<Hex>();

        _directions[target][target] = Hex.zero;
        _distances[target][target] = 0;
        queue.Add(target);

        while (queue.Count > 0)
        {
            queue = queue.OrderBy(node => _distances[target][node]).ToList();
            Hex node = queue.First();
            queue.RemoveAt(0);
            foreach (Hex direction in Hex.directions)
            {
                Hex next = node + direction;
                if (!tiles.ContainsKey(next)) continue;
                if (!tiles[node].HasConnection[direction]) continue;
                if (!tiles[next].HasConnection[-direction]) continue;
                if (visited.Contains(next)) continue;

                if (!_distances[target].ContainsKey(next)
                    || _distances[target][node] + 1 < _distances[target][next])
                {
                    _distances[target][next] = _distances[target][node] + 1;
                    _directions[target][next] = -direction;
                    if (!queue.Contains(next)) queue.Add(next);
                }
            }
            visited.Add(node);
        }
    }
}
