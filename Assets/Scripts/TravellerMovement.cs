using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TravellerMovement
{
    public event Action<Traveller> TravellerAdded;
    public event Action<Traveller> TravellerRemoved;


    private List<Traveller> _travellers = new List<Traveller>();
    private TravellerNavigation _navigation;

    public TravellerMovement(TravellerNavigation navigation)
    {
        _navigation = navigation;
    }

    public void AddTraveller(Hex position, Hex targetPosition)
    {
        var traveller = new Traveller(position, targetPosition);
        _travellers.Add(traveller);
        Debug.Log($"New traveller from {position} to {targetPosition}");
        TravellerAdded?.Invoke(traveller);
    }

    public void Move()
    {
        for (int i = _travellers.Count() - 1; i >= 0; i--)
        {
            var t = _travellers[i];
            bool isStuck = !_navigation.TryGetDirection(t.Position, t.Target, out Hex _);
            if (isStuck) t.IsMoving = false;
            else
            {
                _navigation.TryGetDistance(t.Position, t.Target, out int distance);
                if (distance > 0) MoveTraveller(t);
                else RemoveTraveller(t);
            }
        }
    }

    private void RemoveTraveller(Traveller traveller)
    {
        _travellers.Remove(traveller);
        TravellerRemoved?.Invoke(traveller);
    }

    private void MoveTraveller(Traveller traveller)
    {
        traveller.IsMoving = true;
        _navigation.TryGetDirection(traveller.Position, traveller.Target, out Hex direction);
        traveller.Move(direction);
    }
}
