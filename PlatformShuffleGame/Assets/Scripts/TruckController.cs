using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    [SerializeField] private float _trucksOffset = 5f;
    [SerializeField] private float _truckSpeed = 0.5f;
    private Transform _transform;

    [SerializeField, Space] private TruckMechanics lTruck;
    [SerializeField] private TruckMechanics rTruck;

    [SerializeField, Space] private Transform carStack;
    [SerializeField] private List<GameObject> carPrefabs;

    private Vector2 awaitTrucks = Vector2.zero;

    private bool isMovementNecessary = true;
    private void Awake()
    {
        _transform = transform;


    }
    private void Start()
    {
        GameManager.Instance.ShuffleEvent += ShuffleEventListener;

        if (lTruck != null)
        {
            lTruck.transform.position = new Vector3(-_trucksOffset, lTruck.transform.position.y, lTruck.transform.position.z);
            lTruck.TruckDoneMissionEvent += IsTruckDone;
            CarController tmp = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Count)], carStack, false).GetComponent<CarController>();
            lTruck.PushCar(tmp, true);
            tmp = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Count)], carStack, false).GetComponent<CarController>();
            lTruck.PushCar(tmp, true);
            tmp = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Count)], carStack, false).GetComponent<CarController>();
            lTruck.PushCar(tmp, true);
        }
        if (rTruck != null)
        {
            rTruck.transform.position = new Vector3(_trucksOffset, rTruck.transform.position.y, rTruck.transform.position.z);
            rTruck.TruckDoneMissionEvent += IsTruckDone;
            CarController tmp = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Count)], carStack, false).GetComponent<CarController>();
            rTruck.PushCar(tmp, true);
            tmp = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Count)], carStack, false).GetComponent<CarController>();
            rTruck.PushCar(tmp, true);
            tmp = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Count)], carStack, false).GetComponent<CarController>();
            rTruck.PushCar(tmp, true);
        }
    }
    private void FixedUpdate()
    {
        if(isMovementNecessary)
            _transform.position = _transform.position + Vector3.forward * _truckSpeed;
    }
    private void IsTruckDone(object sender, TruckEventDoneArgs e)
    {
        awaitTrucks += e.TruckSide;
        if (awaitTrucks.Equals(Vector2.one))
            isMovementNecessary = false;
    }

    private void ShuffleEventListener(object sender, ShuffleEventArgs e) 
    {
        if (e.IsShufleRight)
            rTruck.PushCar(lTruck.PopCar());
        else
            lTruck.PushCar(rTruck.PopCar());
    }
}
