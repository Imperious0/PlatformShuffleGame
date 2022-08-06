using System;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    public event EventHandler<EventArgs> TrucksEmptyEvent;
    public event EventHandler<EventArgs> TruckHitsEndLineEvent;
    public event EventHandler<TruckUnloadArgs> TruckUnloadedCargoEvent;

    [SerializeField] private float _trucksOffset = 5f;
    [SerializeField] private float _truckSpeed = 0.5f;

    [SerializeField, Space] private TruckMechanics lTruck;
    [SerializeField] private TruckMechanics rTruck;
    [SerializeField] private TruckLooper looper;
    [SerializeField] private ParticleSystem[] _confetty;

    [SerializeField, Space] private Transform carStack;
    [SerializeField] private List<GameObject> carPrefabs;
    
    private Transform _transform;
    private List<CarController> AvailableCarStack;

    //First Cars Removes Its Truck Index So When Game Starts awaitTrucks became Vector2.zero;
    private Vector2 awaitTrucks = Vector2.one;

    private int _finalizedCarMess = 0;

    private bool isMovementNecessary = true;
    private void Awake()
    {
        _transform = transform;
        AvailableCarStack = new List<CarController>();

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
        GameManager.Instance.ThirdPhaseEvent += TruckUnloadPhaseListener;
        looper.HitEndlineEvent += HitsEndlineListener;
        looper.HitTruckUnloadEvent += HitsTruckUnloadListener;
        if (lTruck != null)
        {
            lTruck.transform.position = new Vector3(-_trucksOffset, lTruck.transform.position.y, lTruck.transform.position.z);
            AddNewCar(lTruck,1);
            AddNewCar(lTruck,1);
            AddNewCar(lTruck,1);
        }
        if (rTruck != null)
        {
            rTruck.transform.position = new Vector3(_trucksOffset, rTruck.transform.position.y, rTruck.transform.position.z);
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

    private void AddNewCar(TruckMechanics t, int Amount)
    {
        CarController tmp = null;
        for (int i = 0; i < Amount; i++)
        {
            if(t==null)
            {
                tmp = Instantiate(carPrefabs[UnityEngine.Random.Range(0, carPrefabs.Count)], carStack, false).GetComponent<CarController>();
                AvailableCarStack.Add(tmp);
                tmp.Die();
                continue;
            }
            if (AvailableCarStack.Count > 0)
            {
                tmp = AvailableCarStack[AvailableCarStack.Count - 1];
                AvailableCarStack.RemoveAt(AvailableCarStack.Count - 1);
                tmp.Respawn();
            }
            else
            {
                AddNewCar(null, Amount + 20);
                AddNewCar(t, Amount);
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
    #region OutsourceListeners
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
    private void TruckUnloadPhaseListener(object sender, EventArgs e)
    {
        isMovementNecessary = true;
    }
    #endregion

    #region InHouseListeners
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
        if(rTruck.CarAmount + lTruck.CarAmount <= 0)
            TrucksEmptyEvent?.Invoke(this, EventArgs.Empty);
    }
    private void HitsEndlineListener(object sender, EventArgs e)
    {
        isMovementNecessary = false;
        TruckMechanics SwizzleTruck = lTruck;
        TruckMechanics SwizzlingTruck = rTruck;
        if (rTruck.CarAmount < lTruck.CarAmount)
        {
            SwizzleTruck = rTruck;
            SwizzlingTruck = lTruck;
        }
        int difference = Mathf.FloorToInt(Math.Abs(SwizzlingTruck.CarAmount - SwizzleTruck.CarAmount) / 2f);
        for (int i = 0; i < difference; i++)
        {
            SwizzleTruck.PushCar(SwizzlingTruck.PopCar());
        }

        TruckHitsEndLineEvent?.Invoke(this, EventArgs.Empty);
        Debug.LogError("Hits Endline");
    }
    private void HitsTruckUnloadListener(object sender, CarUnloadArgs e)
    {
        CarController lCar = lTruck.PopCar();
        CarController rCar = rTruck.PopCar();

        if(rCar != null)
        {
            _finalizedCarMess += rCar.CarMess;
            rCar.transform.SetParent(null, true);
            rCar.MoveNextPos(e.RParkTransform.position);
        }
        if(lCar != null)
        {
            _finalizedCarMess += lCar.CarMess;
            lCar.MoveNextPos(e.LParkTransform.position);
            lCar.transform.SetParent(null, true);
        }
        if (lCar == null && rCar == null || lTruck.CarAmount <= 0 && rTruck.CarAmount <= 0 || e.IsEndUnloader)
        {
            isMovementNecessary = false;
            CarController tmp = null;
            while(lTruck.CarAmount > 0)
            {
                tmp = lTruck.PopCar();
                _finalizedCarMess += tmp.CarMess;
                tmp.Die();
            }
            while (rTruck.CarAmount > 0)
            {
                tmp = rTruck.PopCar();
                _finalizedCarMess += tmp.CarMess;
                tmp.Die();
            }
            TruckUnloadedCargoEvent?.Invoke(this, new TruckUnloadArgs(_finalizedCarMess));
            foreach (ParticleSystem item in _confetty)
            {
                item.Play();
            }
            return;
        }
    }
    #endregion
}
public class TruckUnloadArgs : EventArgs
{
    private int _totalMess = 0;

    public int TotalMess { get => _totalMess; }

    public TruckUnloadArgs(int totalMess)
    {
        _totalMess = totalMess;
    }
}