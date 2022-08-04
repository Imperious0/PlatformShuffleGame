using System;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    public event EventHandler<EventArgs> TrucksEmptyEvent;
    public event EventHandler<EventArgs> TruckHitsEndLineEvent;

    [SerializeField] private float _trucksOffset = 5f;
    [SerializeField] private float _truckSpeed = 0.5f;

    [SerializeField, Space] private TruckMechanics lTruck;
    [SerializeField] private TruckMechanics rTruck;
    [SerializeField] private TruckLooper looper;


    [SerializeField, Space] private Transform carStack;
    [SerializeField] private List<GameObject> carPrefabs;
    
    private Transform _transform;
    private List<CarController> AvailableCarStack;

    //First Cars Removes Its Truck Index So When Game Starts awaitTrucks became Vector2.zero;
    private Vector2 awaitTrucks = Vector2.one;

    private bool isMovementNecessary = true;
    private void Awake()
    {
        _transform = transform;
        AvailableCarStack = new List<CarController>();


        looper.HitEndlineEvent += HitsEndlineListener;
        foreach (var item in GameObject.FindGameObjectsWithTag("Reward"))
        {
            IPrize p = item.GetComponent(typeof(IPrize)) as IPrize;
            if (p != null)
                p.PrizeCollectedEvent += RewardCollectedEventListener;
        }

    }
    private void Start()
    {
        GameManager.Instance.ShuffleEvent += ShuffleEventListener;
        GameManager.Instance.GameOverEvent += GameOverListener;
        if (lTruck != null)
        {
            lTruck.transform.position = new Vector3(-_trucksOffset, lTruck.transform.position.y, lTruck.transform.position.z);
            lTruck.TruckDoneMissionEvent += IsTruckDone;
            AddNewCar(lTruck,1);
            AddNewCar(lTruck,1);
            AddNewCar(lTruck,1);
        }
        if (rTruck != null)
        {
            rTruck.transform.position = new Vector3(_trucksOffset, rTruck.transform.position.y, rTruck.transform.position.z);
            rTruck.TruckDoneMissionEvent += IsTruckDone;
            AddNewCar(rTruck,1);
            AddNewCar(rTruck,1);
            AddNewCar(rTruck,1);
        }
    }
    private void FixedUpdate()
    {
        if(isMovementNecessary)
            _transform.position = _transform.position + Vector3.forward * _truckSpeed;
    }
    private void ShuffleEventListener(object sender, ShuffleEventArgs e)
    {
        if (e.IsShufleRight)
            rTruck.PushCar(lTruck.PopCar());
        else
            lTruck.PushCar(rTruck.PopCar());
    }

    private void GameOverListener(object sender, EventArgs e) 
    {
        isMovementNecessary = false;
    }
    private void IsTruckDone(object sender, TruckEventDoneArgs e)
    {
        awaitTrucks += e.TruckSide;
        if (awaitTrucks.Equals(Vector2.one))
        {
            isMovementNecessary = false;
            TrucksEmptyEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    private void HitsEndlineListener(object sender, EventArgs e)
    {
        Debug.LogError("Hits Endline");
        TruckHitsEndLineEvent?.Invoke(this, EventArgs.Empty);
    }

    private void AddNewCar(TruckMechanics t, int Amount)
    {
        CarController tmp = null;
        for (int i = 0; i < Amount; i++)
        {
            if (AvailableCarStack.Count > 0)
            {
                tmp = AvailableCarStack[AvailableCarStack.Count - 1];
                AvailableCarStack.RemoveAt(AvailableCarStack.Count - 1);
                tmp.Respawn();
            }
            else
            {
                tmp = Instantiate(carPrefabs[UnityEngine.Random.Range(0, carPrefabs.Count)], carStack, false).GetComponent<CarController>();
            }

            t.PushCar(tmp, true);
        }

    }
    private void RemoveCar(TruckMechanics t, int Amount)
    {
        CarController tmp = null;
        for (int i = 0; i < Amount; i++)
        {
            tmp = t.PopCar();
            if (tmp == null)
                break;

            AvailableCarStack.Add(tmp);
            tmp.Die();
        }
        
    }
    private void RewardCollectedEventListener(object sender, PrizeCollectedArgs e)
    {
        TruckMechanics t = null;
        if (e.TruckIndex == 0)
            t = lTruck;
        else
            t = rTruck;

        switch (e.PType)
        {
            case PrizeType.ADD:
                AddNewCar(t, e.PrizeAmount);
                break;
            case PrizeType.REMOVE:
                RemoveCar(t, e.PrizeAmount);
                break;
            case PrizeType.MUL:
                int estamiedAmount = t.CarAmount * e.PrizeAmount;
                estamiedAmount -= t.CarAmount;
                AddNewCar(t, estamiedAmount);
                break;
            case PrizeType.DIV:
                float estamiedAmount2 = t.CarAmount / e.PrizeAmount;
                estamiedAmount2 = Mathf.FloorToInt(estamiedAmount2);
                estamiedAmount2 = t.CarAmount - (int)estamiedAmount2;
                RemoveCar(t, (int)estamiedAmount2);
                break;
        }
    }
}
