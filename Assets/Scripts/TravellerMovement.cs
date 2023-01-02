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
        var travellersStuck = _travellers.Where(t => !_navigation.TryGetDirection(t.Position, t.Target, out Hex _));
        foreach (var traveller in travellersStuck)
            traveller.IsMoving = false;

        var travellersNotStuck = _travellers.Where(t => _navigation.TryGetDirection(t.Position, t.Target, out Hex _));
        foreach (var traveller in travellersNotStuck)
        {
            traveller.IsMoving = true;
            _navigation.TryGetDirection(traveller.Position, traveller.Target, out Hex direction);
            traveller.Move(direction);
            if (direction != Hex.zero)
                Debug.Log($"Traveller moved from {traveller.Position} in direction {direction} to target {traveller.Target}");
        }
    }
}
