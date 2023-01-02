using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerView : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private GameObject _movingVisual;
    [SerializeField] private GameObject _waitingVisual;

    private Vector2 _targetposition;

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, _targetposition, _speed * Time.deltaTime);
    }

    public void SetStartPosition(Vector2 startPosition)
    {
        _targetposition = startPosition;
        transform.position = startPosition;
    } 

    public void SetPosition(Vector2 position) => _targetposition = position;

    public void SetWaiting()
    {
        _movingVisual.SetActive(false);
        _waitingVisual.SetActive(true);
    }

    public void SetMoving()
    {
        _movingVisual.SetActive(true);
        _waitingVisual.SetActive(false);
    }
}
