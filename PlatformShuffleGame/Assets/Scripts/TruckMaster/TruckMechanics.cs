using System;
using System.Collections.Generic;
using UnityEngine;

public class TruckMechanics : MonoBehaviour
{

    [SerializeField] private Vector2 _index;
    [SerializeField] private Vector3 _carStartPoint = Vector3.zero;
    [SerializeField] private Vector3 _carPositionOffset = Vector3.zero;

    List<CarController> _cars;
    public int CarAmount { get => _cars.Count; }

    private void Awake()
    {
        _cars = new List<CarController>();
    }
    public CarController PopCar()
    {
        if (_cars.Count <= 0)
            return null;

        CarController tmp = _cars[_cars.Count - 1];
        _cars.RemoveAt(_cars.Count - 1);

        return tmp;
    }
    public void PushCar(CarController t, bool isSpawn = false)
    {
        if (t == null)
            return;

        int row = _cars.Count / 3;
        int col = _cars.Count % 3;

        Vector3 newPosition = _carStartPoint;
        newPosition += new Vector3(0f, row * _carPositionOffset.y, col * _carPositionOffset.z);

        if (!isSpawn)
            t.MoveNextPos(newPosition);
        else
            t.InitializePosition(newPosition);
        _cars.Add(t);
    }

}
